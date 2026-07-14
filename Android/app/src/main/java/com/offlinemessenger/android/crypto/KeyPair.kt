package com.offlinemessenger.android.crypto

import java.security.PrivateKey


class KeyPair {

    lateinit var privateKey: PrivateKey

    lateinit var publicKey: ByteArray
}