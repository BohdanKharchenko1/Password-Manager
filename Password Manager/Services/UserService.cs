using System;
using System.IO;
using System.Threading.Tasks;
using Password_Manager.Models;

namespace Password_Manager.Services
{
    public static class UserService
    {
        private const string DirectoryPath = "Users";

        /// <summary>
        /// Registers a new user by hashing the master password and creating an encrypted empty entries file.
        /// </summary>
        /// <param name="userName">The username for the new user.</param>
        /// <param name="masterPassword">The master password to be hashed and used for encryption.</param>
        /// <returns>A UserModel on success, or null if the user already exists or an error occurs.</returns>
        public static async Task<UserModel?> RegisterUserAsync(string userName, string masterPassword)
        {
            try
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                
                string hashFilePath = Path.Combine(DirectoryPath, $"{userName}_master.hash");
                if (File.Exists(hashFilePath))
                {
                    return null;
                }
                
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(masterPassword);
                await File.WriteAllTextAsync(hashFilePath, hashedPassword);
                
                string entriesFilePath = Path.Combine(DirectoryPath, $"{userName}.json");
                string initialData = "[]";
                string encryptedData = EncryptionService.Encrypt(initialData, masterPassword);
                await File.WriteAllTextAsync(entriesFilePath, encryptedData);
                
                return new UserModel
                {
                    UserName = userName,
                    MasterPassword = masterPassword,
                    EntriesFilePath = entriesFilePath
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Verifies a user's login credentials by comparing the entered master password with the stored hash.
        /// </summary>
        /// <param name="userName">The username to verify.</param>
        /// <param name="masterPassword">The master password to check against the stored hash.</param>
        /// <returns>A UserModel on success, or null if the user does not exist, the password is incorrect, or an error occurs.</returns>
        public static async Task<UserModel?> VerifyUserAsync(string userName, string masterPassword)
        {
            try
            {
                string hashFilePath = Path.Combine(DirectoryPath, $"{userName}_master.hash");

                if (!File.Exists(hashFilePath))
                {
                    return null;
                }
                
                string hashedPassword = await File.ReadAllTextAsync(hashFilePath);
                if (!BCrypt.Net.BCrypt.Verify(masterPassword, hashedPassword))
                {
                    return null;
                }

                string entriesFilePath = Path.Combine(DirectoryPath, $"{userName}.json");
                if (!File.Exists(entriesFilePath))
                {
                    string initialData = "[]";
                    string encryptedData = EncryptionService.Encrypt(initialData, masterPassword);
                    await File.WriteAllTextAsync(entriesFilePath, encryptedData);
                }
                
                return new UserModel
                {
                    UserName = userName,
                    MasterPassword = masterPassword,
                    EntriesFilePath = entriesFilePath
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}