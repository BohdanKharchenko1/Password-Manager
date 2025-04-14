using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Password_Manager.Services
{
    public static class EncryptionService
    {
        /// <summary>
        /// Encrypts plain text using AES and a key derived from the provided password.
        /// A random salt and IV are generated and prepended to the encrypted data.
        /// The resulting data is returned as a Base64-encoded string.
        /// </summary>
        /// <param name="plainText">Text to encrypt.</param>
        /// <param name="password">User's master password.</param>
        /// <returns>Base64 string containing the salt, IV, and encrypted data.</returns>
        public static string Encrypt(string plainText, string password)
        {
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Generate a random salt (16 bytes)
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            
            // Derive the key using the random salt
            byte[] key = DeriveKey(password, salt, aes.KeySize / 8);
            aes.Key = key;
            
            // Generate a random IV.
            aes.GenerateIV();
            byte[] iv = aes.IV;
            
            using MemoryStream ms = new MemoryStream();
            // Write the salt and IV at the beginning of the stream.
            ms.Write(salt, 0, salt.Length);
            ms.Write(iv, 0, iv.Length);
            
            // Encrypt the plain text.
            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            
            // Get the complete encrypted data from the memory stream.
            byte[] encryptedBytes = ms.ToArray();
            
            // Return the encrypted data as a Base64 string.
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts the Base64 encoded cipher text (which includes the salt and IV)
        /// using the user's password.
        /// </summary>
        /// <param name="cipherText">The Base64 encoded string containing the salt, IV, and encrypted data.</param>
        /// <param name="password">User's master password.</param>
        /// <returns>The decrypted plain text.</returns>
        public static string Decrypt(string cipherText, string password)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            // The salt is the first 16 bytes.
            byte[] salt = new byte[16];
            Array.Copy(fullCipher, 0, salt, 0, salt.Length);
            
            // The IV is next.
            int ivLength = aes.BlockSize / 8; // Typically 16 bytes for AES.
            byte[] iv = new byte[ivLength];
            Array.Copy(fullCipher, salt.Length, iv, 0, ivLength);
            
            // The remaining bytes are the cipher text.
            int cipherTextLength = fullCipher.Length - salt.Length - ivLength;
            byte[] cipher = new byte[cipherTextLength];
            Array.Copy(fullCipher, salt.Length + ivLength, cipher, 0, cipherTextLength);
            
            // Derive the key using the extracted salt.
            byte[] key = DeriveKey(password, salt, aes.KeySize / 8);
            aes.Key = key;
            aes.IV = iv;
            
            using MemoryStream ms = new MemoryStream(cipher);
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Derives a cryptographic key from the given password and salt using PBKDF2.
        /// </summary>
        /// <param name="password">The user's master password.</param>
        /// <param name="salt">The salt to use in the key derivation.</param>
        /// <param name="keyBytes">The desired key length in bytes.</param>
        /// <returns>The derived key as a byte array.</returns>
        private static byte[] DeriveKey(string password, byte[] salt, int keyBytes)
        {
            // Use PBKDF2 with 10,000 iterations and SHA256.
            using var pdb = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            return pdb.GetBytes(keyBytes);
        }
    }
}
