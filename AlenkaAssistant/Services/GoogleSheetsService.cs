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
            /// <summary>
            /// Column mapping. Set to null or 0 to exclude a column from being sent to Google Sheets.
            /// For example: { "assistantNames": null } or { "assistantNames": 0 } will not send assistant names.
            /// </summary>
            public Dictionary<string, int?> ColumnMapping { get; set; } = new Dictionary<string, int?>
            {
                { "year", 1 },
                { "month", 2 },
                { "date", 3 },
                { "totalCost", 4 },
                { "costDetail", 5 },
                { "userId", 6 },
                { "treatmentDescription", 7 },
                { "treatmentType", 8 },
                { "assistantNames", 9 },
                { "doctorName", 10 }
            };
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

                // Get column positions from config
                var colMap = _config.ColumnMapping;

                // Prepare rows for each cost item
                var rows = new List<List<object>>();

                if (request.CostDetails != null && request.CostDetails.Count > 0)
                {
                    for (int i = 0; i < request.CostDetails.Count; i++)
                    {
                        var costItem = request.CostDetails[i];
                        var row = BuildRowWithColumnMapping(
                            colMap,
                            i == 0 ? year : "",
                            i == 0 ? month : "",
                            i == 0 ? date : "",
                            i == 0 ? totalCost : "",
                            costItem.Cost.ToString(),
                            i == 0 ? request.UserId : "",
                            costItem.TreatmentDesc ?? "",
                            i == 0 ? treatmentType : "",
                            i == 0 ? assistantNames : "",
                            i == 0 ? doctorName : ""
                        );
                        rows.Add(row);
                    }
                }
                else
                {
                    // If no cost details, create one row with empty cost
                    var row = BuildRowWithColumnMapping(
                        colMap,
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
                    );
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
        /// Build a row based on column mapping configuration
        /// </summary>
        private List<object> BuildRowWithColumnMapping(
            Dictionary<string, int?> columnMapping,
            string year,
            string month,
            string date,
            string totalCost,
            string costDetail,
            string userId,
            string treatmentDescription,
            string treatmentType,
            string assistantNames,
            string doctorName)
        {
            // Find the maximum column position to know how large the row should be
            // Skip columns with null or 0 values (both mean "don't use this column")
            int maxCol = columnMapping.Values
                .Where(v => v.HasValue && v.Value > 0)
                .DefaultIfEmpty(0)
                .Max() ?? 0;

            if (maxCol == 0)
            {
                // No columns configured, return empty row
                return new List<object>();
            }
            var row = new List<object>(new object[maxCol]);

            // Helper function to safely set value at column position
            Action<string, string> SetColumn = (key, value) =>
            {
                // Skip if key not found, value is null, or value is 0
                if (columnMapping.ContainsKey(key) && columnMapping[key].HasValue && columnMapping[key].Value > 0)
                {
                    int colPos = columnMapping[key].Value - 1; // Convert 1-based to 0-based
                    if (colPos >= 0 && colPos < row.Count)
                    {
                        row[colPos] = value ?? "";
                    }
                }
            };

            // Map all values based on column configuration
            SetColumn("year", year);
            SetColumn("month", month);
            SetColumn("date", date);
            SetColumn("totalCost", totalCost);
            SetColumn("costDetail", costDetail);
            SetColumn("userId", userId);
            SetColumn("treatmentDescription", treatmentDescription);
            SetColumn("treatmentType", treatmentType);
            SetColumn("assistantNames", assistantNames);
            SetColumn("doctorName", doctorName);

            // Replace any nulls with empty strings
            for (int i = 0; i < row.Count; i++)
            {
                if (row[i] == null)
                    row[i] = "";
            }

            return row;
        }

        /// <summary>
        /// Check if service is properly configured and enabled
        /// </summary>
        public bool IsConfigured => _httpClient != null && _config?.Enabled == true;
    }
}
