package com.offlinemessenger.android.crypto


import java.security.KeyFactory
import java.security.KeyPairGenerator
import java.security.PrivateKey
import java.security.spec.X509EncodedKeySpec
import javax.crypto.KeyAgreement


class KeyExchangeService {


    fun generateKeyPair(): KeyPair {


        val generator =
            KeyPairGenerator.getInstance(
                "EC"
            )


        generator.initialize(
            256
        )


        val keyPair =
            generator.generateKeyPair()


        return KeyPair().apply {

            privateKey =
                keyPair.private

            publicKey =
                keyPair.public.encoded
        }
    }




    fun deriveSharedSecret(
        privateKey: PrivateKey,
        remotePublicKey: ByteArray
    ): ByteArray {


        val keyFactory =
            KeyFactory.getInstance(
                "EC"
            )


        val publicKey =
            keyFactory.generatePublic(
                X509EncodedKeySpec(
                    remotePublicKey
                )
            )



        val agreement =
            KeyAgreement.getInstance(
                "ECDH"
            )


        agreement.init(
            privateKey
        )


        agreement.doPhase(
            publicKey,
            true
        )


        return agreement.generateSecret()
    }
}