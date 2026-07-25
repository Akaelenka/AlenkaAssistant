using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AlenkaAssistant.Models;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Service for managing local data persistence using JSON
    /// </summary>
    public class LocalDataService
    {
        private readonly string _dataDirectory;
        private readonly string _dataFilePath;
        private const string DATA_FOLDER = "Data";
        private const string DATA_FILE = "local_purchases.json";

        public LocalDataService()
        {
            // Store data in AppData\Local\AlenkaAssistant
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AlenkaAssistant",
                DATA_FOLDER);

            _dataFilePath = Path.Combine(_dataDirectory, DATA_FILE);

            // Ensure directory exists
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        /// <summary>
        /// Save a purchase request to local storage
        /// </summary>
        public async Task<bool> SavePurchaseRequestAsync(PurchaseRequestModel request)
        {
            try
            {
                // Ensure ID is set
                if (request.Id == 0)
                {
                    request.Id = GenerateUniqueId();
                }

                // Ensure timestamp is set
                if (request.CreatedAt == default)
                {
                    request.CreatedAt = DateTime.Now;
                }

                // Load existing records
                var records = await LoadAllPurchaseRequestsAsync();

                // Check if this is an update (same ID exists)
                var existingIndex = records.FindIndex(r => r.Id == request.Id);
                if (existingIndex >= 0)
                {
                    records[existingIndex] = request;
                }
                else
                {
                    records.Add(request);
                }

                // Save back to file
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };

                var json = JsonSerializer.Serialize(records, options);
                await File.WriteAllTextAsync(_dataFilePath, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving purchase request: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load all purchase requests from local storage
        /// </summary>
        public async Task<List<PurchaseRequestModel>> LoadAllPurchaseRequestsAsync()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return new List<PurchaseRequestModel>();
                }

                var json = await File.ReadAllTextAsync(_dataFilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var records = JsonSerializer.Deserialize<List<PurchaseRequestModel>>(json, options);
                return records ?? new List<PurchaseRequestModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading purchase requests: {ex.Message}");
                return new List<PurchaseRequestModel>();
            }
        }

        /// <summary>
        /// Load the most recent purchase request
        /// </summary>
        public async Task<PurchaseRequestModel?> LoadLastPurchaseRequestAsync()
        {
            try
            {
                var records = await LoadAllPurchaseRequestsAsync();
                return records.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading last purchase request: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Load a specific purchase request by ID
        /// </summary>
        public async Task<PurchaseRequestModel?> LoadPurchaseRequestByIdAsync(int id)
        {
            try
            {
                var records = await LoadAllPurchaseRequestsAsync();
                return records.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading purchase request with ID {id}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Delete a purchase request by ID
        /// </summary>
        public async Task<bool> DeletePurchaseRequestAsync(int id)
        {
            try
            {
                var records = await LoadAllPurchaseRequestsAsync();
                var recordToRemove = records.FirstOrDefault(r => r.Id == id);

                if (recordToRemove != null)
                {
                    records.Remove(recordToRemove);

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNameCaseInsensitive = true
                    };

                    var json = JsonSerializer.Serialize(records, options);
                    await File.WriteAllTextAsync(_dataFilePath, json);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting purchase request: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate a unique ID for a new record
        /// </summary>
        private int GenerateUniqueId()
        {
            return (int)(DateTime.Now.Ticks % int.MaxValue);
        }

        /// <summary>
        /// Get the path where local data is stored (useful for debugging)
        /// </summary>
        public string GetDataFilePath()
        {
            return _dataFilePath;
        }
    }
}
