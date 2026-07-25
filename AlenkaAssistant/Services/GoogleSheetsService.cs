using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AlenkaAssistant.Models;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Service for interacting with Google Sheets via Google Apps Script
    /// </summary>
    public class GoogleSheetsService
    {
        private HttpClient _httpClient;
        private GoogleSheetsConfig _config;

        /// <summary>
        /// Configuration class for Google Sheets
        /// </summary>
        public class GoogleSheetsConfig
        {
            public string DeploymentUrl { get; set; }
            public string SpreadsheetId { get; set; }
            public string SheetName { get; set; }
            public bool Enabled { get; set; }
        }

        /// <summary>
        /// Initialize Google Sheets Service with config
        /// </summary>
        public async Task<bool> InitializeAsync(string configPath)
        {
            try
            {
                // Load config from JSON file
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Config file not found: {configPath}");
                }

                var json = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _config = JsonSerializer.Deserialize<GoogleSheetsConfig>(json, options);

                if (_config == null || !_config.Enabled)
                {
                    return false;
                }

                // Validate deployment URL
                if (string.IsNullOrEmpty(_config.DeploymentUrl))
                {
                    throw new InvalidOperationException("DeploymentUrl is not configured in GoogleSheetsConfig.json");
                }

                if (!Uri.TryCreate(_config.DeploymentUrl, UriKind.Absolute, out _))
                {
                    throw new InvalidOperationException($"DeploymentUrl is not a valid URL: {_config.DeploymentUrl}");
                }

                // Initialize HTTP client
                _httpClient = new HttpClient();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize Google Sheets service: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get Indonesian month name
        /// </summary>
        private string GetIndonesianMonth(int month)
        {
            return month switch
            {
                1 => "JANUARI",
                2 => "FEBRUARI",
                3 => "MARET",
                4 => "APRIL",
                5 => "MEI",
                6 => "JUNI",
                7 => "JULI",
                8 => "AGUSTUS",
                9 => "SEPTEMBER",
                10 => "OKTOBER",
                11 => "NOVEMBER",
                12 => "DESEMBER",
                _ => ""
            };
        }

        /// <summary>
        /// Get treatment type display name
        /// </summary>
        private string GetTreatmentTypeDisplay(TreatmentType? type)
        {
            if (!type.HasValue)
                return "";

            // Use the helper to get display name (replaces _ with space)
            return TreatmentTypeHelper.GetDisplayName(type.Value);
        }

        /// <summary>
        /// Get assistant names as comma-separated string
        /// </summary>
        private string GetAssistantNames(List<string> altAssistantNames)
        {
            if (altAssistantNames == null || altAssistantNames.Count == 0)
                return "";

            return string.Join("/", altAssistantNames);
        }

        /// <summary>
        /// Get doctor name display
        /// </summary>
        private string GetDoctorDisplay(DoctorName? doctor)
        {
            if (!doctor.HasValue)
                return "";

            return DoctorNameHelper.GetDisplayName(doctor.Value);
        }

        /// <summary>
        /// Append purchase request data to Google Sheet
        /// </summary>
        public async Task<bool> AppendPurchaseRequestAsync(PurchaseRequestModel request)
        {
            try
            {
                if (_httpClient == null || _config == null)
                {
                    throw new InvalidOperationException("Google Sheets service not initialized");
                }

                DateTime dateTime = request.CreatedAt;
                string year = dateTime.Year.ToString();
                string month = GetIndonesianMonth(dateTime.Month);
                string date = dateTime.Day.ToString();
                string totalCost = request.TotalCost.ToString();
                string treatmentType = GetTreatmentTypeDisplay(request.TreatmentType);
                string assistantNames = GetAssistantNames(request.AltAssistantName);
                string doctorName = GetDoctorDisplay(request.DoctorName);

                // Prepare rows for each cost item
                var rows = new List<List<object>>();

                if (request.CostDetails != null && request.CostDetails.Count > 0)
                {
                    for (int i = 0; i < request.CostDetails.Count; i++)
                    {
                        var costItem = request.CostDetails[i];
                        var row = new List<object>
                        {
                            i == 0 ? year : "",                          // Year (only first row)
                            i == 0 ? month : "",                         // Month (only first row)
                            i == 0 ? date : "",                          // Date (only first row)
                            i == 0 ? totalCost : "",                     // Total Cost (only first row)
                            costItem.Cost.ToString(),                    // Cost Detail
                            i == 0 ? request.UserId : "",                // RM# (only first row)
                            costItem.TreatmentDesc ?? "",                // Tindakan
                            i == 0 ? treatmentType : "",                 // Treatment Type (only first row)
                            i == 0 ? assistantNames : "",                // Assistant Name (only first row)
                            i == 0 ? doctorName : ""                     // Doctor Name (only first row)
                        };
                        rows.Add(row);
                    }
                }
                else
                {
                    // If no cost details, create one row with empty cost
                    var row = new List<object>
                    {
                        year,
                        month,
                        date,
                        totalCost,
                        "",
                        request.UserId,
                        request.GeneralTreatmentDesc ?? "",
                        treatmentType,
                        assistantNames,
                        doctorName
                    };
                    rows.Add(row);
                }

                // Prepare JSON payload
                var payload = new
                {
                    sheetName = _config.SheetName,
                    values = rows
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send POST request to Google Apps Script
                var response = await _httpClient.PostAsync(_config.DeploymentUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Failed to append to Google Sheet. Status: {response.StatusCode}. Response: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // Check if response is empty or starts with HTML (error)
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    throw new Exception("Empty response from Google Apps Script");
                }

                if (responseContent.StartsWith("<") || responseContent.StartsWith("<!"))
                {
                    throw new Exception($"Google Apps Script returned HTML instead of JSON. This usually means the deployment URL is invalid or the script has an error. Response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
                }

                var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (responseObj.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                {
                    return true;
                }
                else
                {
                    var errorMsg = responseObj.TryGetProperty("error", out var errorProp) 
                        ? errorProp.GetString() 
                        : "Unknown error";
                    throw new Exception($"Google Apps Script returned error: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to append purchase request to Google Sheet: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if service is properly configured and enabled
        /// </summary>
        public bool IsConfigured => _httpClient != null && _config?.Enabled == true;
    }
}
