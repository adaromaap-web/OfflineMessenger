using System.Text;
using OfflineMessenger.Crypto;
using Xunit;
using System.Security.Cryptography;

namespace OfflineMessenger.Tests;

public class CryptoTests
{
    //AES-GCM: Шифрование данных

    [Fact]
    public void EncryptThenDecrypt_ReturnsOriginalData()
    {
        var crypto = new CryptoService();

        var key = new byte[32];

        for (int i = 0; i < key.Length; i++)
        {
            key[i] = (byte)i;
        }


        var originalMessage =
            "Hello Bluetooth";

        var data =
            Encoding.UTF8.GetBytes(originalMessage);


        var encrypted =
            crypto.Encrypt(
                key,
                data,
                out var nonce,
                out var tag
            );


        var decrypted =
            crypto.Decrypt(
                key,
                encrypted,
                nonce,
                tag
            );


        var result =
            Encoding.UTF8.GetString(decrypted);


        Assert.Equal(
            originalMessage,
            result
        );
    }

    // Шифрование и последующая расшифровка возвращают исходные данные
    // Расшифровка с неверным ключом вызывает ошибку

    [Fact]
    public void DecryptWithWrongKey_ThrowsException()
    {
        var crypto = new CryptoService();


        var correctKey = new byte[32];

        for (int i = 0; i < correctKey.Length; i++)
        {
            correctKey[i] = (byte)i;
        }


        var wrongKey = new byte[32];

        for (int i = 0; i < wrongKey.Length; i++)
        {
            wrongKey[i] = (byte)(i + 1);
        }


        var data =
            Encoding.UTF8.GetBytes("Secret message");


        var cipher =
            crypto.Encrypt(
                correctKey,
                data,
                out var nonce,
                out var tag
            );


        Assert.Throws<AuthenticationTagMismatchException>(() =>
        {
            crypto.Decrypt(
                wrongKey,
                cipher,
                nonce,
                tag
            );
        });
    }

    // HKDF: Генерация ключа сессии

    [Fact]
    public void Hkdf_WithSameSecret_ReturnsSameKey()
    {
        var secret =
            Encoding.UTF8.GetBytes("shared secret");


        var key1 =
            Hkdf.DeriveKey(secret);


        var key2 =
            Hkdf.DeriveKey(secret);


        Assert.Equal(
            key1,
            key2
        );
    }

    // Одинаковый секрет создаёт одинаковый ключ
    // Разные секреты создают разные ключи

    [Fact]
    public void Hkdf_WithDifferentSecrets_ReturnsDifferentKeys()
    {
        var secret1 =
            Encoding.UTF8.GetBytes("secret one");


        var secret2 =
            Encoding.UTF8.GetBytes("secret two");


        var key1 =
            Hkdf.DeriveKey(secret1);


        var key2 =
            Hkdf.DeriveKey(secret2);


        Assert.NotEqual(
            key1,
            key2
        );
    }

    // ECDH: Обмен ключами

    [Fact]
    public void KeyExchange_BothSidesDeriveSameSecret()
    {
        var exchange = new KeyExchangeService();


        var alice =
            exchange.GenerateKeyPair();


        var bob =
            exchange.GenerateKeyPair();


        var aliceSecret =
            exchange.DeriveSharedSecret(
                alice.PrivateKey,
                bob.PublicKey
            );


        var bobSecret =
            exchange.DeriveSharedSecret(
                bob.PrivateKey,
                alice.PublicKey
            );


        Assert.Equal(
            aliceSecret,
            bobSecret
        );
    }

    // Обе стороны получают одинаковый общий секрет
    // Разные участники получают разные общие секреты

    [Fact]
    public void KeyExchange_DifferentPeersProduceDifferentSecrets()
    {
        var exchange = new KeyExchangeService();


        var alice =
            exchange.GenerateKeyPair();


        var bob =
            exchange.GenerateKeyPair();


        var charlie =
            exchange.GenerateKeyPair();


        var secretAB =
            exchange.DeriveSharedSecret(
                alice.PrivateKey,
                bob.PublicKey
            );


        var secretAC =
            exchange.DeriveSharedSecret(
                alice.PrivateKey,
                charlie.PublicKey
            );


        Assert.NotEqual(
            secretAB,
            secretAC
        );
    }
}