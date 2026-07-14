package com.offlinemessenger.android.crypto

import java.security.MessageDigest


object Hkdf {


    fun deriveKey(
        sharedSecret: ByteArray
    ): ByteArray {


        val digest =
            MessageDigest.getInstance(
                "SHA-256"
            )


        return digest.digest(
            sharedSecret
        )
    }
}