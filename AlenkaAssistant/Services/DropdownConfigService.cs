using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Service for managing dropdown configuration from GoogleSheetsConfig.json
    /// </summary>
    public class DropdownConfigService
    {
        private List<string> _assistantNames = new();
        private List<string> _doctorNames = new();
        private string _deploymentUrl = string.Empty;
        private string _noRmSpreadsheetId = string.Empty;
        private string _noRmSheetName = "NoRM";
        private string _paperSize = "A4";
        private int _rmColumn = 0;
        private int _patientNameColumn = 1;

        /// <summary>
        /// Load dropdown configuration from GoogleSheetsConfig.json
        /// </summary>
        public async Task<bool> LoadDropdownConfigAsync(string configPath)
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    return false;
                }

                var json = await File.ReadAllTextAsync(configPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var config = JsonSerializer.Deserialize<JsonElement>(json, options);

                // Load deployment URL
                if (config.TryGetProperty("deploymentUrl", out var deploymentUrlElement))
                {
                    _deploymentUrl = deploymentUrlElement.GetString() ?? string.Empty;
                }

                // Load NoRM spreadsheet ID
                if (config.TryGetProperty("noRmSpreadsheetId", out var noRmSpreadsheetIdElement))
                {
                    _noRmSpreadsheetId = noRmSpreadsheetIdElement.GetString() ?? string.Empty;
                }

                // Load assistant names
                if (config.TryGetProperty("assistantNames", out var assistantNamesElement))
                {
                    _assistantNames.Clear();
                    if (assistantNamesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in assistantNamesElement.EnumerateArray())
                        {
                            if (item.GetString() is string name && !string.IsNullOrWhiteSpace(name))
                            {
                                _assistantNames.Add(name);
                            }
                        }
                    }
                }

                // Load doctor names
                if (config.TryGetProperty("doctorNames", out var doctorNamesElement))
                {
                    _doctorNames.Clear();
                    if (doctorNamesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in doctorNamesElement.EnumerateArray())
                        {
                            if (item.GetString() is string name && !string.IsNullOrWhiteSpace(name))
                            {
                                _doctorNames.Add(name);
                            }
                        }
                    }
                }

                // Load paper size
                if (config.TryGetProperty("paperSize", out var paperSizeElement))
                {
                    var paperSize = paperSizeElement.GetString();
                    if (!string.IsNullOrWhiteSpace(paperSize))
                    {
                        _paperSize = paperSize;
                    }
                }

                // Load NoRM sheet name from patientLookup
                if (config.TryGetProperty("patientLookup", out var patientLookupElement))
                {
                    if (patientLookupElement.TryGetProperty("noRmSheetName", out var noRmSheetNameElement))
                    {
                        var noRmSheetName = noRmSheetNameElement.GetString();
                        if (!string.IsNullOrWhiteSpace(noRmSheetName))
                        {
                            _noRmSheetName = noRmSheetName;
                        }
                    }

                    // Load RM and patient name columns
                    if (patientLookupElement.TryGetProperty("rmColumn", out var rmColumnElement))
                    {
                        if (rmColumnElement.TryGetInt32(out var rmCol))
                        {
                            _rmColumn = rmCol;
                        }
                    }

                    if (patientLookupElement.TryGetProperty("patientNameColumn", out var patientNameColumnElement))
                    {
                        if (patientNameColumnElement.TryGetInt32(out var patientNameCol))
                        {
                            _patientNameColumn = patientNameCol;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dropdown config: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get observable collection of assistant names for UI binding
        /// </summary>
        public ObservableCollection<string> GetAssistantNames()
        {
            var collection = new ObservableCollection<string>(_assistantNames);

            // Ensure "Other" is at the end if not already present
            if (!collection.Contains("Other"))
            {
                collection.Add("Other");
            }
            else if (collection.IndexOf("Other") != collection.Count - 1)
            {
                collection.Remove("Other");
                collection.Add("Other");
            }

            return collection;
        }

        /// <summary>
        /// Get observable collection of doctor names for UI binding
        /// </summary>
        public ObservableCollection<string> GetDoctorNames()
        {
            var collection = new ObservableCollection<string>(_doctorNames);

            // Ensure "Other" is at the end if not already present
            if (!collection.Contains("Other"))
            {
                collection.Add("Other");
            }
            else if (collection.IndexOf("Other") != collection.Count - 1)
            {
                collection.Remove("Other");
                collection.Add("Other");
            }

            return collection;
        }

        /// <summary>
        /// Get raw list of assistant names
        /// </summary>
        public List<string> GetAssistantNamesList()
        {
            return new List<string>(_assistantNames);
        }

        /// <summary>
        /// Get raw list of doctor names
        /// </summary>
        public List<string> GetDoctorNamesList()
        {
            return new List<string>(_doctorNames);
        }

        /// <summary>
        /// Get configured paper size for printing
        /// </summary>
        public string GetPaperSize()
        {
            return _paperSize;
        }

        /// <summary>
        /// Get deployment URL for Google Apps Script
        /// </summary>
        public string GetDeploymentUrl()
        {
            return _deploymentUrl;
        }

        /// <summary>
        /// Get NoRM spreadsheet ID
        /// </summary>
        public string GetNoRmSpreadsheetId()
        {
            return _noRmSpreadsheetId;
        }

        /// <summary>
        /// Get NoRM sheet name
        /// </summary>
        public string GetNoRmSheetName()
        {
            return _noRmSheetName;
        }

        /// <summary>
        /// Get RM column index for NoRM spreadsheet
        /// </summary>
        public int GetRmColumn()
        {
            return _rmColumn;
        }

        /// <summary>
        /// Get patient name column index for NoRM spreadsheet
        /// </summary>
        public int GetPatientNameColumn()
        {
            return _patientNameColumn;
        }
    }
}
