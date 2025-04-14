using System;
using System.IO;
using System.Threading.Tasks;
using Password_Manager.Models;       // UserModel should be defined here.

namespace Password_Manager.Services
{
    public static class UserService
    {
        private const string DirectoryPath = "Users";

        /// <summary>
        /// Registers a new user by hashing the master password and creating an encrypted empty entries file.
        /// Returns a UserModel on success, or null if the user already exists or an error occurs.
        /// </summary>
        public static async Task<UserModel?> RegisterUserAsync(string userName, string masterPassword)
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                
                // Path for the hashed master password file
                string hashFilePath = Path.Combine(DirectoryPath, $"{userName}_master.hash");
                if (File.Exists(hashFilePath))
                {
                    // User already exists.
                    return null;
                }
                
                // Hash and store the master password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(masterPassword);
                await File.WriteAllTextAsync(hashFilePath, hashedPassword);
                
                // Create an empty encrypted entries file for the user
                string entriesFilePath = Path.Combine(DirectoryPath, $"{userName}.json");
                string initialData = "[]"; // empty JSON array
                string encryptedData = EncryptionService.Encrypt(initialData, masterPassword);
                await File.WriteAllTextAsync(entriesFilePath, encryptedData);
                
                // Return a new UserModel with necessary information
                return new UserModel
                {
                    UserName = userName,
                    MasterPassword = masterPassword, // Stored in memory only during the session!
                    EntriesFilePath = entriesFilePath
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Verifies a user's login credentials by comparing the entered master password with the stored hash.
        /// If successful, returns a UserModel; otherwise, returns null.
        /// </summary>
        public static async Task<UserModel?> VerifyUserAsync(string userName, string masterPassword)
        {
            try
            {
                string hashFilePath = Path.Combine(DirectoryPath, $"{userName}_master.hash");

                if (!File.Exists(hashFilePath))
                {
                    // User does not exist.
                    return null;
                }
                
                string hashedPassword = await File.ReadAllTextAsync(hashFilePath);
                if (!BCrypt.Net.BCrypt.Verify(masterPassword, hashedPassword))
                {
                    // The master password is incorrect.
                    return null;
                }

                // Ensure the user's encrypted entries file exists; if not, create it.
                string entriesFilePath = Path.Combine(DirectoryPath, $"{userName}.json");
                if (!File.Exists(entriesFilePath))
                {
                    string initialData = "[]";
                    string encryptedData = EncryptionService.Encrypt(initialData, masterPassword);
                    await File.WriteAllTextAsync(entriesFilePath, encryptedData);
                }
                
                // Return a new UserModel that holds session-specific data
                return new UserModel
                {
                    UserName = userName,
                    MasterPassword = masterPassword, // Keep master password only in memory as needed!
                    EntriesFilePath = entriesFilePath
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
