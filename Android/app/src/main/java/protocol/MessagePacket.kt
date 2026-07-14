package com.offlinemessenger.android.protocol

import java.util.UUID


class MessagePacket {

    var type: MessageType = MessageType.Chat

    var sessionId: UUID = UUID.randomUUID()

    var messageId: UUID = UUID.randomUUID()

    var timestamp: Long = 0

    var payload: ByteArray = ByteArray(0)

    var nonce: ByteArray = ByteArray(0)

    var tag: ByteArray = ByteArray(0)
}