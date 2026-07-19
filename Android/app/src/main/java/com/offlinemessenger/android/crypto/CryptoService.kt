package com.offlinemessenger.android.crypto

import javax.crypto.Cipher
import javax.crypto.spec.GCMParameterSpec
import javax.crypto.spec.SecretKeySpec
import java.security.SecureRandom


class CryptoService {


    fun decrypt(
        key: ByteArray,
        encrypted: ByteArray,
        nonce: ByteArray,
        tag: ByteArray
    ): ByteArray {


        val cipher =
            Cipher.getInstance(
                "AES/GCM/NoPadding"
            )


        val secretKey =
            SecretKeySpec(
                key,
                "AES"
            )


        val spec =
            GCMParameterSpec(
                128,
                nonce
            )


        cipher.init(
            Cipher.DECRYPT_MODE,
            secretKey,
            spec
        )


        val combined =
            ByteArray(
                encrypted.size + tag.size
            )


        System.arraycopy(
            encrypted,
            0,
            combined,
            0,
            encrypted.size
        )


        System.arraycopy(
            tag,
            0,
            combined,
            encrypted.size,
            tag.size
        )


        return cipher.doFinal(
            combined
        )
    }

    fun encrypt(
        key: ByteArray,
        data: ByteArray
    ): EncryptedData {

        val nonce = ByteArray(12)

        SecureRandom().nextBytes(nonce)


        val cipher =
            Cipher.getInstance(
                "AES/GCM/NoPadding"
            )


        val secretKey =
            SecretKeySpec(
                key,
                "AES"
            )


        val spec =
            GCMParameterSpec(
                128,
                nonce
            )


        cipher.init(
            Cipher.ENCRYPT_MODE,
            secretKey,
            spec
        )


        val encrypted =
            cipher.doFinal(data)


        val tagSize = 16


        val payload =
            encrypted.copyOfRange(
                0,
                encrypted.size - tagSize
            )


        val tag =
            encrypted.copyOfRange(
                encrypted.size - tagSize,
                encrypted.size
            )


        return EncryptedData(
            payload = payload,
            nonce = nonce,
            tag = tag
        )
    }
}

data class EncryptedData(
    val payload: ByteArray,
    val nonce: ByteArray,
    val tag: ByteArray
)