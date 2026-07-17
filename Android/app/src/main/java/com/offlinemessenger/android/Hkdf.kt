package com.offlinemessenger.android.crypto

import android.util.Log
import java.security.MessageDigest

object Hkdf {

    fun deriveKey(
        sharedSecret: ByteArray
    ): ByteArray {

        Log.d(
            "HKDF",
            "I AM HERE"
        )

        Log.d(
            "HKDF",
            "INPUT: ${
                sharedSecret.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )

        val digest =
            MessageDigest.getInstance("SHA-256")

        val result =
            digest.digest(sharedSecret)

        Log.d(
            "HKDF",
            "OUTPUT: ${
                result.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )

        return result
    }
}