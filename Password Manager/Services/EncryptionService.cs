using System;
using System.IO;
using System.Security.Cryptography;

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
            try
            {
                using Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] salt = new byte[16];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);

                byte[] key = DeriveKey(password, salt, aes.KeySize / 8);
                aes.Key = key;

                aes.GenerateIV();
                byte[] iv = aes.IV;

                using var ms = new MemoryStream();
                ms.Write(salt, 0, salt.Length);
                ms.Write(iv, 0, iv.Length);

                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                throw new Exception("Encryption failed.", e);
            }
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
            try
            {
                byte[] fullCipher = Convert.FromBase64String(cipherText);

                using Aes aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] salt = new byte[16];
                Array.Copy(fullCipher, 0, salt, 0, salt.Length);

                int ivLength = aes.BlockSize / 8;
                byte[] iv = new byte[ivLength];
                Array.Copy(fullCipher, salt.Length, iv, 0, ivLength);

                int cipherTextLength = fullCipher.Length - salt.Length - ivLength;
                byte[] cipher = new byte[cipherTextLength];
                Array.Copy(fullCipher, salt.Length + ivLength, cipher, 0, cipherTextLength);

                byte[] key = DeriveKey(password, salt, aes.KeySize / 8);
                aes.Key = key;
                aes.IV = iv;

                using var ms = new MemoryStream(cipher);
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch (Exception e)
            {
                throw new Exception("Decryption failed.", e);
            }
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
            using var pdb = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            return pdb.GetBytes(keyBytes);
        }
    }
}
