using System.Security.Cryptography;
using System.Text;

namespace OfflineMessenger.Crypto;

public static class Hkdf
{
    public static byte[] DeriveKey(byte[] sharedSecret)
    {
        // простой вариант (пока без полноценного HKDF RFC)
        using var sha = SHA256.Create();
        return sha.ComputeHash(sharedSecret);
    }
}