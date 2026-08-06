package com.offlinemessenger.android.crypto

import org.junit.Assert
import org.junit.Test

class CryptoServiceTest {

    @Test
    fun encryptDecrypt_ReturnsOriginalData() {

        val crypto =
            CryptoService()


        val key =
            ByteArray(32) {
                it.toByte()
            }


        val original =
            "Привет Android".toByteArray()


        val encrypted =
            crypto.encrypt(
                key,
                original
            )


        val decrypted =
            crypto.decrypt(
                key,
                encrypted.payload,
                encrypted.nonce,
                encrypted.tag
            )


        Assert.assertArrayEquals(
            original,
            decrypted
        )
    }
}