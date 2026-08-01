using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Response from getting the last RM number
    /// </summary>
    public class GetLastRmResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("lastRm")]
        public string LastRm { get; set; }

        [JsonPropertyName("nextRm")]
        public string NextRm { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    /// <summary>
    /// Response from adding a new patient
    /// </summary>
    public class AddPatientResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("rm")]
        public string Rm { get; set; }

        [JsonPropertyName("patientName")]
        public string PatientName { get; set; }

        [JsonPropertyName("rowAdded")]
        public int RowAdded { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    /// <summary>
    /// Service for managing patient creation and RM auto-increment
    /// </summary>
    public class AddPatientService
    {
        private readonly string _deploymentUrl;
        private readonly string _patientLookupSheetName;
        private readonly int _rmColumn;
        private readonly int _patientNameColumn;
        private readonly HttpClient _httpClient;

        public AddPatientService(
            string deploymentUrl,
            string patientLookupSheetName = "NoRM",
            int rmColumn = 0,
            int patientNameColumn = 1)
        {
            _deploymentUrl = deploymentUrl;
            _patientLookupSheetName = patientLookupSheetName;
            _rmColumn = rmColumn;
            _patientNameColumn = patientNameColumn;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Get the last RM number from the patient lookup sheet and suggest the next one
        /// </summary>
        /// <returns>Response containing last RM and next suggested RM</returns>
        public async Task<GetLastRmResponse> GetLastRmAsync()
        {
            try
            {
                string queryUrl = $"{_deploymentUrl}?action=getLastRm&sheetName={Uri.EscapeDataString(_patientLookupSheetName)}&rmColumn={_rmColumn}";

                System.Diagnostics.Debug.WriteLine($"[AddPatientService] Getting last RM from: {queryUrl}");

                var response = await _httpClient.GetAsync(queryUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return new GetLastRmResponse
                    {
                        Success = false,
                        Error = $"HTTP error: {response.StatusCode}"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[AddPatientService] Response: {content}");

                if (string.IsNullOrWhiteSpace(content))
                {
                    return new GetLastRmResponse
                    {
                        Success = false,
                        Error = "Empty response from server"
                    };
                }

                var result = JsonSerializer.Deserialize<GetLastRmResponse>(content);
                return result ?? new GetLastRmResponse
                {
                    Success = false,
                    Error = "Failed to parse response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AddPatientService] Exception in GetLastRmAsync: {ex.Message}");
                return new GetLastRmResponse
                {
                    Success = false,
                    Error = $"Exception: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Add a new patient to the patient lookup sheet
        /// </summary>
        /// <param name="rmNumber">RM number (e.g., "A.0001")</param>
        /// <param name="patientName">Patient name</param>
        /// <returns>Response confirming patient was added</returns>
        public async Task<AddPatientResponse> AddPatientAsync(string rmNumber, string patientName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rmNumber) || string.IsNullOrWhiteSpace(patientName))
                {
                    return new AddPatientResponse
                    {
                        Success = false,
                        Error = "RM number and patient name are required"
                    };
                }

                // Build query URL
                string queryUrl = $"{_deploymentUrl}?action=addPatient&sheetName={Uri.EscapeDataString(_patientLookupSheetName)}&rm={Uri.EscapeDataString(rmNumber)}&patientName={Uri.EscapeDataString(patientName)}&rmColumn={_rmColumn}&patientNameColumn={_patientNameColumn}";

                System.Diagnostics.Debug.WriteLine($"[AddPatientService] Adding patient: {rmNumber} - {patientName}");

                var response = await _httpClient.GetAsync(queryUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return new AddPatientResponse
                    {
                        Success = false,
                        Error = $"HTTP error: {response.StatusCode}"
                    };
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[AddPatientService] Add response: {content}");

                if (string.IsNullOrWhiteSpace(content))
                {
                    return new AddPatientResponse
                    {
                        Success = false,
                        Error = "Empty response from server"
                    };
                }

                var result = JsonSerializer.Deserialize<AddPatientResponse>(content);
                return result ?? new AddPatientResponse
                {
                    Success = false,
                    Error = "Failed to parse response"
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AddPatientService] Exception in AddPatientAsync: {ex.Message}");
                return new AddPatientResponse
                {
                    Success = false,
                    Error = $"Exception: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Increment an RM number by 1
        /// Supports formats like "A.0001" (increment the numeric part to "A.0002")
        /// </summary>
        /// <param name="rmNumber">Current RM number</param>
        /// <returns>Next RM number, or null if format is invalid</returns>
        public static string IncrementRmNumber(string rmNumber)
        {
            if (string.IsNullOrWhiteSpace(rmNumber))
                return null;

            try
            {
                // Handle format like "A.0001"
                if (rmNumber.Contains("."))
                {
                    var parts = rmNumber.Split('.');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int number))
                    {
                        int nextNumber = number + 1;
                        return $"{parts[0]}.{nextNumber:D4}"; // Format as 4 digits with leading zeros
                    }
                }

                // Handle plain numeric format
                if (int.TryParse(rmNumber, out int plainNumber))
                {
                    return (plainNumber + 1).ToString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
