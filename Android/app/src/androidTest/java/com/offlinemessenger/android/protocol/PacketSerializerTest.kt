package com.offlinemessenger.android.protocol

import org.junit.Test
import org.junit.Assert.assertEquals
import org.junit.Assert.assertArrayEquals
import java.util.UUID


class PacketSerializerTest {


    @Test
    fun messagePacket_SerializeDeserialize_PreservesData() {

        // Проверяет сериализацию сетевого пакета.
        // После преобразования MessagePacket -> ByteArray -> MessagePacket
        // все данные должны сохраниться.


        val original =
            MessagePacket()


        val sessionId =
            UUID.randomUUID()

        val messageId =
            UUID.randomUUID()


        original.type =
            MessageType.Chat


        original.sessionId =
            sessionId


        original.messageId =
            messageId


        original.timestamp =
            123456789L


        original.payload =
            "Hello Android".toByteArray()


        original.nonce =
            byteArrayOf(
                1, 2, 3, 4
            )


        original.tag =
            byteArrayOf(
                5, 6, 7, 8
            )



        val bytes =
            PacketSerializer.serialize(
                original
            )


        val restored =
            PacketSerializer.deserialize(
                bytes
            )



        assertEquals(
            original.type,
            restored.type
        )


        assertEquals(
            original.sessionId,
            restored.sessionId
        )


        assertEquals(
            original.messageId,
            restored.messageId
        )


        assertEquals(
            original.timestamp,
            restored.timestamp
        )


        assertArrayEquals(
            original.payload,
            restored.payload
        )


        assertArrayEquals(
            original.nonce,
            restored.nonce
        )


        assertArrayEquals(
            original.tag,
            restored.tag
        )
    }
}