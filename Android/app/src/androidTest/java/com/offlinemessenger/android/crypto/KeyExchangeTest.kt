package com.offlinemessenger.android.crypto

import org.junit.Assert
import org.junit.Test

class KeyExchangeTest {


    @Test
    fun ecdh_TwoSidesProduceSameSharedSecret() {


        val service =
            KeyExchangeService()


        val alice =
            service.generateKeyPair()


        val bob =
            service.generateKeyPair()



        val aliceSecret =
            service.deriveSharedSecret(
                alice.privateKey,
                bob.publicKey
            )



        val bobSecret =
            service.deriveSharedSecret(
                bob.privateKey,
                alice.publicKey
            )



        Assert.assertArrayEquals(
            aliceSecret,
            bobSecret
        )
    }
}