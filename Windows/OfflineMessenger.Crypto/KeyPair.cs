using System.Security.Cryptography;

namespace OfflineMessenger.Crypto;

public class KeyPair
{
    public ECDiffieHellman PrivateKey { get; set; }
    public byte[] PublicKey { get; set; }
}