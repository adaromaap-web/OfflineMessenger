using System.Security.Cryptography;

namespace OfflineMessenger.Crypto;

public class CryptoService : ICryptoService
{
    public byte[] Encrypt(byte[] key, byte[] data, out byte[] nonce, out byte[] tag)
    {
        nonce = RandomNumberGenerator.GetBytes(12);
        tag = new byte[16];

        var cipher = new byte[data.Length];

        using var aes = new AesGcm(key);
        aes.Encrypt(nonce, data, cipher, tag);

        return cipher;
    }

    public byte[] Decrypt(byte[] key, byte[] cipher, byte[] nonce, byte[] tag)
    {
        var plain = new byte[cipher.Length];

        using var aes = new AesGcm(key);
        aes.Decrypt(nonce, cipher, tag, plain);

        return plain;
    }
}