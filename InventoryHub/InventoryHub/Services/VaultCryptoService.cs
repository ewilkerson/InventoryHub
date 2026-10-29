using System.Security.Cryptography;
using System.Text;

namespace InventoryHub.Services
{
    public class VaultCryptoService
    {
        private const int Keysize = 256;
        private const int DerivationIterations = 100_000;

        public static string Encrypt(string plainText, string masterPassword)
        {
            // Generate a random 16-byte Salt and a 16-byte Initialization Vector (IV)
            byte[] saltStringBytes = RandomNumberGenerator.GetBytes(16);
            byte[] ivStringBytes = RandomNumberGenerator.GetBytes(16);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = Rfc2898DeriveBytes.Pbkdf2(
    masterPassword,
    saltStringBytes,
    DerivationIterations,
    HashAlgorithmName.SHA256,
    Keysize / 8
);

            using var symmetricKey = Aes.Create();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;

            using var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            // Prepend Salt and IV to the cipher text so they can be parsed out during decryption
            byte[] cipherTextBytes = saltStringBytes;
            cipherTextBytes = [.. cipherTextBytes, .. ivStringBytes];
            cipherTextBytes = [.. cipherTextBytes, .. memoryStream.ToArray()];

            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText, string masterPassword)
        {
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv[..16];
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv[16..32];
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv[32..];

            byte[] keyBytes = Rfc2898DeriveBytes.Pbkdf2(
    masterPassword,
    saltStringBytes,
    DerivationIterations,
    HashAlgorithmName.SHA256,
    Keysize / 8
);
            using var symmetricKey = Aes.Create();
            symmetricKey.BlockSize = 128;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;

            using var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
            using var memoryStream = new MemoryStream(cipherTextBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}
