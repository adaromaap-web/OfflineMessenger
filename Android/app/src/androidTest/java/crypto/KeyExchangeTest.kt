package com.offlinemessenger.android.crypto

import org.junit.Assert.assertArrayEquals
import org.junit.Test

// Проверяет обмен ключами ECDH.
// Два участника должны получить одинаковый общий секрет после обмена публичными ключами.

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



        assertArrayEquals(
            aliceSecret,
            bobSecret
        )
    }
}