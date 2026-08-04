using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlenkaAssistant.Services
{
    public class PatientLookupResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("found")]
        public bool Found { get; set; }

        [JsonPropertyName("patientName")]
        public string PatientName { get; set; }

        [JsonPropertyName("rmValue")]
        public string RmValue { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    public class PatientLookupService
    {
        private readonly string _deploymentUrl;
        private readonly string _noRmSpreadsheetId;
        private readonly string _noRmSheetName;
        private readonly HttpClient _httpClient;

        public PatientLookupService(string deploymentUrl, string noRmSpreadsheetId = null, string noRmSheetName = "NoRM")
        {
            _deploymentUrl = deploymentUrl;
            _noRmSpreadsheetId = noRmSpreadsheetId;
            _noRmSheetName = noRmSheetName;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Lookup patient name by RM number
        /// </summary>
        /// <param name="sheetName">Sheet name to search in (defaults to configured NoRM sheet name)</param>
        /// <param name="rmNumber">RM number (can be in format "1001" or "A.1001")</param>
        /// <param name="searchColumn">Column index to search (0-based)</param>
        /// <param name="resultColumn">Column index for result (0-based)</param>
        /// <returns>PatientLookupResponse with patient name or error</returns>
        public async Task<PatientLookupResponse> LookupPatientAsync(
            string sheetName = null,
            string rmNumber = null,
            int searchColumn = 0,
            int resultColumn = 1)
        {
            // Use configured noRmSheetName if sheetName not provided
            sheetName = sheetName ?? _noRmSheetName;

            try
            {
                if (string.IsNullOrWhiteSpace(rmNumber))
                {
                    return new PatientLookupResponse
                    {
                        Success = false,
                        Error = "RM number is empty"
                    };
                }

                // Extract just the number part if input is "A.1001" format
                string searchValue = rmNumber;
                if (rmNumber.Contains("."))
                {
                    var parts = rmNumber.Split('.');
                    searchValue = parts[parts.Length - 1]; // Get the last part after the dot
                }

                // Build query URL
                string queryUrl = $"{_deploymentUrl}?action=lookup&sheetName={Uri.EscapeDataString(sheetName)}&searchColumn={searchColumn}&searchValue={Uri.EscapeDataString(searchValue)}&resultColumn={resultColumn}";

                if (!string.IsNullOrWhiteSpace(_noRmSpreadsheetId))
                {
                    queryUrl += $"&spreadsheetId={Uri.EscapeDataString(_noRmSpreadsheetId)}";
                }

                System.Diagnostics.Debug.WriteLine($"[PatientLookupService] Querying: {queryUrl}");

                var response = await _httpClient.GetAsync(queryUrl);

                if (!response.IsSuccessStatusCode)
                {
                    // Provide specific error messages based on status code
                    string errorMsg = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.NotFound => 
                            "Deployment URL not found (404). Please verify: 1) The Google Apps Script deployment URL in GoogleSheetsConfig.json is correct and up-to-date, 2) The deployment hasn't been deleted, 3) Try redeploying the script and updating the URL.",
                        System.Net.HttpStatusCode.Forbidden => 
                            "Access forbidden (403). Check if the Google Apps Script deployment allows 'Anyone' access.",
                        System.Net.HttpStatusCode.BadRequest => 
                            "Bad request (400). Check if all parameters are valid.",
                        System.Net.HttpStatusCode.ServiceUnavailable => 
                            "Google service unavailable (503). Try again in a moment.",
                        _ => $"HTTP error: {response.StatusCode}"
                    };

                    return new PatientLookupResponse
                    {
                        Success = false,
                        Error = errorMsg
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[PatientLookupService] Response: {content}");

                if (string.IsNullOrWhiteSpace(content))
                {
                    return new PatientLookupResponse
                    {
                        Success = false,
                        Error = "Empty response from server"
                    };
                }

                var result = JsonSerializer.Deserialize<PatientLookupResponse>(content);
                return result ?? new PatientLookupResponse
                {
                    Success = false,
                    Error = "Failed to parse response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PatientLookupService] Exception: {ex.Message}");
                return new PatientLookupResponse
                {
                    Success = false,
                    Error = $"Exception: {ex.Message}"
                };
            }
        }
    }
}
