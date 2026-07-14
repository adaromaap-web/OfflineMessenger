package com.offlinemessenger.android.protocol


class HandshakePacket {

    var type: MessageType =
        MessageType.HandshakeInit

    var publicKey: ByteArray =
        ByteArray(0)
}