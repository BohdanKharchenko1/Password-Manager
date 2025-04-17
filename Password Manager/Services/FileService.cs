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
        /// Loads and decrypts the password entries stored in the specified file.
        /// </summary>
        /// <param name="path">The file path to the encrypted JSON file.</param>
        /// <param name="masterPassword">The master password used to decrypt the file.</param>
        /// <returns>An IEnumerable of PasswordEntryModel or null if an error occurs.</returns>
        public static async Task<IEnumerable<PasswordEntryModel>?> LoadEncryptedEntriesAsync(string path,
            string masterPassword)
        {
            if (!File.Exists(path))
            {
                // If the file doesn't exist, return an empty list.
                return new List<PasswordEntryModel>();
            }

            try
            {
                // Read the entire encrypted data from disk.
                string encryptedData = await File.ReadAllTextAsync(path);

                // Decrypt the data using the provided master password.
                string decryptedJson = EncryptionService.Decrypt(encryptedData, masterPassword);

                // Deserialize the decrypted JSON into a collection of entries.
                return JsonSerializer.Deserialize<IEnumerable<PasswordEntryModel>>(decryptedJson);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading entries: " + e);
                return null;
            }
        }

        /// <summary>
        /// Saves a new password entry to the user's encrypted JSON file.
        /// If the file exists, it decrypts and deserializes the current data,
        /// adds the new entry, then re-serializes and encrypts it.
        /// </summary>
        /// <param name="entry">The new password entry to add.</param>
        /// <param name="path">The file path to the encrypted JSON file.</param>
        /// <param name="masterPassword">The master password used to encrypt the file.</param>
        /// <returns>True if the operation succeeds; otherwise, false.</returns>
        public static async Task<bool> SaveEncryptedEntryAsync(PasswordEntryModel entry, string path,
            string masterPassword)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            List<PasswordEntryModel> entries = new();

            try
            {
                if (File.Exists(path))
                {
                    // Read existing encrypted data
                    string encryptedData = await File.ReadAllTextAsync(path);
                    string decryptedJson = EncryptionService.Decrypt(encryptedData, masterPassword);
                    var existingEntries = JsonSerializer.Deserialize<List<PasswordEntryModel>>(decryptedJson);
                    if (existingEntries != null)
                        entries = existingEntries;
                }

                var existing = entries.FirstOrDefault(e => e.Id == entry.Id);
                if (existing != null)
                {
                    // Replace the existing entry
                    int index = entries.IndexOf(existing);
                    entries[index] = entry;
                }
                else
                {
                    // It's a new entry
                    entries.Add(entry);
                }

                // Serialize updated list
                string jsonContent = JsonSerializer.Serialize(entries);
                string encryptedContent = EncryptionService.Encrypt(jsonContent, masterPassword);
                await File.WriteAllTextAsync(path, encryptedContent);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error saving entry: " + e);
                return false;
            }
        }
        public static async Task<bool> DeleteEncryptedEntryAsync(Guid entryId, string path, string masterPassword)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            List<PasswordEntryModel> entries = new();

            try
            {
                if (File.Exists(path))
                {
                    // Read and decrypt existing data.
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
                Console.WriteLine("Error deleting entry: " + e);
                return false;
            }
        }
    
    }
}
