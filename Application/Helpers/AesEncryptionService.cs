using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers;

public sealed class AesEncryptionService
{
    private const byte Version = 1;
    private const int KeySizeBytes = 32;
    private const int NonceSizeBytes = 12;
    private const int TagSizeBytes = 16;
    private const int OverheadBytes = 1 + NonceSizeBytes + TagSizeBytes;

    private readonly byte[] _key;

    public AesEncryptionService(string key)
    {
        _key = ParseKey(key);
    }

    public byte[] EncryptToBytes(string plainText)
    {
        if (plainText is null)
        {
            throw new ArgumentNullException(nameof(plainText));
        }

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
        var ciphertext = new byte[plainBytes.Length];
        var tag = new byte[TagSizeBytes];

        using (var aesGcm = new AesGcm(_key, TagSizeBytes))
        {
            aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);
        }

        var result = new byte[OverheadBytes + ciphertext.Length];
        result[0] = Version;
        Buffer.BlockCopy(nonce, 0, result, 1, NonceSizeBytes);
        Buffer.BlockCopy(tag, 0, result, 1 + NonceSizeBytes, TagSizeBytes);
        Buffer.BlockCopy(ciphertext, 0, result, 1 + NonceSizeBytes + TagSizeBytes, ciphertext.Length);

        CryptographicOperations.ZeroMemory(plainBytes);

        return result;
    }

    public string Encrypt(string plainText, int? maxOutputChars = null)
    {
        var payload = EncryptToBytes(plainText);
        var encoded = Convert.ToBase64String(payload);

        if (maxOutputChars.HasValue && encoded.Length > maxOutputChars.Value)
        {
            throw new CryptographicException($"Encrypted output length ({encoded.Length}) exceeds the configured maximum ({maxOutputChars.Value}).");
        }

        return encoded;
    }

    public string Decrypt(string cipherText)
    {
        if (cipherText is null)
        {
            throw new ArgumentNullException(nameof(cipherText));
        }

        return DecryptFromBytes(Convert.FromBase64String(cipherText));
    }

    public string DecryptFromBytes(byte[] fullCipher)
    {
        if (fullCipher is null)
        {
            throw new ArgumentNullException(nameof(fullCipher));
        }

        if (fullCipher.Length < OverheadBytes)
        {
            throw new CryptographicException("Invalid cipher payload.");
        }

        if (fullCipher[0] != Version)
        {
            throw new CryptographicException("Unsupported cipher payload version.");
        }

        var nonce = new byte[NonceSizeBytes];
        var tag = new byte[TagSizeBytes];
        var ciphertext = new byte[fullCipher.Length - OverheadBytes];

        Buffer.BlockCopy(fullCipher, 1, nonce, 0, NonceSizeBytes);
        Buffer.BlockCopy(fullCipher, 1 + NonceSizeBytes, tag, 0, TagSizeBytes);
        Buffer.BlockCopy(fullCipher, 1 + NonceSizeBytes + TagSizeBytes, ciphertext, 0, ciphertext.Length);

        var plainBytes = new byte[ciphertext.Length];

        using (var aesGcm = new AesGcm(_key, TagSizeBytes))
        {
            aesGcm.Decrypt(nonce, ciphertext, tag, plainBytes);
        }

        var plainText = Encoding.UTF8.GetString(plainBytes);
        CryptographicOperations.ZeroMemory(plainBytes);

        return plainText;
    }

    public static string GenerateKeyBase64()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(KeySizeBytes));
    }

    public static int MaxPlainTextUtf8BytesForMaxBase64Chars(int maxBase64Chars)
    {
        if (maxBase64Chars <= 0)
        {
            return 0;
        }

        var maxTriplets = maxBase64Chars / 4;
        var maxBytes = maxTriplets * 3;
        var maxPlainBytes = maxBytes - OverheadBytes;
        return maxPlainBytes <= 0 ? 0 : maxPlainBytes;
    }

    private static byte[] ParseKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Encryption key is required.", nameof(key));
        }

        try
        {
            var keyBytes = Convert.FromBase64String(key);
            if (keyBytes.Length == KeySizeBytes)
            {
                return keyBytes;
            }
        }
        catch (FormatException)
        {
        }

        var utf8 = Encoding.UTF8.GetBytes(key);
        if (utf8.Length != KeySizeBytes)
        {
            throw new ArgumentException("AES-256 key must be exactly 32 bytes (either Base64 for 32 bytes, or a 32-byte UTF-8 string).", nameof(key));
        }

        return utf8;
    }
}
