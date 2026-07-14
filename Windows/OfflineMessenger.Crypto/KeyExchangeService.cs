using System.Security.Cryptography;

namespace OfflineMessenger.Crypto;

public class KeyExchangeService
{
    public KeyPair GenerateKeyPair()
    {
        var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

        return new KeyPair
        {
            PrivateKey = ecdh,
            PublicKey = ecdh.PublicKey.ExportSubjectPublicKeyInfo()
        };
    }

    public byte[] DeriveSharedSecret(ECDiffieHellman privateKey, byte[] remotePublicKey)
    {
        using var remote = ECDiffieHellman.Create();
        remote.ImportSubjectPublicKeyInfo(remotePublicKey, out _);

        return privateKey.DeriveKeyMaterial(remote.PublicKey);
    }
}