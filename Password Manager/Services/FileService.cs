using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Password_Manager.Models;

namespace Password_Manager.Services
{
    public static class FileService
    {
        /// <summary>
        /// Loads and decrypts password entries from the specified file, returning them as an enumerable collection.
        /// </summary>
        /// <param name="path">Path to the encrypted JSON file.</param>
        /// <param name="masterPassword">Master password for decryption.</param>
        /// <returns> Enumerable of PasswordEntryModel or null if an error occurs.</returns>
        public static async Task<IEnumerable<PasswordEntryModel>?> LoadEncryptedEntriesAsync(string path, string masterPassword)
        {
            if (!File.Exists(path))
            {
                return new List<PasswordEntryModel>();
            }

            try
            {
                string encryptedData = await File.ReadAllTextAsync(path);
                string decryptedJson = EncryptionService.Decrypt(encryptedData, masterPassword);
                return JsonSerializer.Deserialize<IEnumerable<PasswordEntryModel>>(decryptedJson);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading entries: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves a password entry to an encrypted JSON file, updating or adding it to existing entries.
        /// </summary>
        /// <param name="entry">Password entry to save.</param>
        /// <param name="path">Path to the encrypted JSON file.</param>
        /// <param name="masterPassword">Master password for encryption.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static async Task<bool> SaveEncryptedEntryAsync(PasswordEntryModel entry, string path, string masterPassword)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            List<PasswordEntryModel> entries = new();

            try
            {
                if (File.Exists(path))
                {
                    string encryptedData = await File.ReadAllTextAsync(path);
                    string decryptedJson = EncryptionService.Decrypt(encryptedData, masterPassword);
                    var existingEntries = JsonSerializer.Deserialize<List<PasswordEntryModel>>(decryptedJson);
                    if (existingEntries != null)
                        entries = existingEntries;
                }

                var existing = entries.FirstOrDefault(e => e.Id == entry.Id);
                if (existing != null)
                {
                    int index = entries.IndexOf(existing);
                    entries[index] = entry;
                }
                else
                {
                    entries.Add(entry);
                }

                string jsonContent = JsonSerializer.Serialize(entries);
                string encryptedContent = EncryptionService.Encrypt(jsonContent, masterPassword);
                await File.WriteAllTextAsync(path, encryptedContent);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving entries: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a password entry from the encrypted JSON file by its ID.
        /// </summary>
        /// <param name="entryId">ID of the entry to delete.</param>
        /// <param name="path">Path to the encrypted JSON file.</param>
        /// <param name="masterPassword">Master password for decryption and encryption.</param>
        /// <returns>True if successful, false if the entry is not found or an error occurs.</returns>
        public static async Task<bool> DeleteEncryptedEntryAsync(Guid entryId, string path, string masterPassword)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            List<PasswordEntryModel> entries = new();

            try
            {
                if (File.Exists(path))
                {
                    string encryptedData = await File.ReadAllTextAsync(path);
                    string decryptedJson = EncryptionService.Decrypt(encryptedData, masterPassword);
                    var existingEntries = JsonSerializer.Deserialize<List<PasswordEntryModel>>(decryptedJson);
                    if (existingEntries != null)
                        entries = existingEntries;
                }

                var entryToRemove = entries.FirstOrDefault(e => e.Id == entryId);
                if (entryToRemove == null)
                {
                    Console.WriteLine("Entry to delete was not found.");
                    return false;
                }

                entries.Remove(entryToRemove);

                string jsonContent = JsonSerializer.Serialize(entries);
                string encryptedContent = EncryptionService.Encrypt(jsonContent, masterPassword);
                await File.WriteAllTextAsync(path, encryptedContent);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting entries: {e.Message}");
                return false;
            }
        }
    }
}