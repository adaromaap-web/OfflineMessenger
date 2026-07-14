using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Crypto;

public interface ICryptoService
{
    byte[] Encrypt(byte[] key, byte[] data, out byte[] nonce, out byte[] tag);
    byte[] Decrypt(byte[] key, byte[] cipher, byte[] nonce, byte[] tag);
}