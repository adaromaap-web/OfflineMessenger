using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMessenger.Crypto;

public interface IKeyExchangeService
{
    KeyPair GenerateKeyPair();

    SessionKeys CreateSessionKeys(
        byte[] myPrivateKey,
        byte[] remotePublicKey);
}