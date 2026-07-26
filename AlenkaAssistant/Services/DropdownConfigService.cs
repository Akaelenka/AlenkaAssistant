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
        /// Get deployment URL for Google Apps Script
        /// </summary>
        public string GetDeploymentUrl()
        {
            return _deploymentUrl;
        }
    }
}
