using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AlenkaAssistant.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Service for interacting with Google Sheets API to save purchase request data
    /// </summary>
    public class GoogleSheetsService
    {
        private SheetsService _sheetsService;
        private GoogleSheetsConfig _config;
        private const string ApplicationName = "AlenkaAssistant";

        /// <summary>
        /// Configuration class for Google Sheets
        /// </summary>
        public class GoogleSheetsConfig
        {
            public string CredentialsPath { get; set; }
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

                if (!File.Exists(_config.CredentialsPath))
                {
                    throw new FileNotFoundException($"Credentials file not found: {_config.CredentialsPath}");
                }

                // Authenticate with Google Sheets API
                GoogleCredential credential;
                using (var stream = new FileStream(_config.CredentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(SheetsService.Scope.Spreadsheets);
                }

                _sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

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
                if (_sheetsService == null || _config == null)
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
                var rows = new List<IList<object>>();

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

                // Append rows to sheet
                var valueRange = new ValueRange()
                {
                    Values = rows.Cast<IList<object>>().ToList()
                };

                var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, _config.SpreadsheetId, $"{_config.SheetName}!A:J");
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

                var appendResponse = await appendRequest.ExecuteAsync();

                return appendResponse != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to append purchase request to Google Sheet: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Check if service is properly configured and enabled
        /// </summary>
        public bool IsConfigured => _sheetsService != null && _config?.Enabled == true;
    }
}
