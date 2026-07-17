package com.offlinemessenger.android.protocol

import java.io.ByteArrayInputStream
import java.io.ByteArrayOutputStream
import java.io.DataInputStream
import java.io.DataOutputStream
import java.util.UUID
import android.util.Log

object PacketSerializer {


    fun serialize(packet: MessagePacket): ByteArray {

        val output = ByteArrayOutputStream()
        val writer = DataOutputStream(output)


        writer.writeByte(
            packet.type.ordinal + 1
        )

        writer.write(
            uuidToBytes(packet.sessionId)
        )

        writer.write(
            uuidToBytes(packet.messageId)
        )

        writer.writeLong(
            packet.timestamp
        )


        writer.writeInt(
            packet.nonce.size
        )

        writer.write(
            packet.nonce
        )


        writer.writeInt(
            packet.tag.size
        )

        writer.write(
            packet.tag
        )


        writer.writeInt(
            packet.payload.size
        )

        writer.write(
            packet.payload
        )


        writer.flush()

        return output.toByteArray()
    }



    fun deserialize(data: ByteArray): MessagePacket {

        val input =
            DataInputStream(
                ByteArrayInputStream(data)
            )

        Log.d(
            "SERIALIZER",
            data.joinToString(
                prefix = "",
                separator = " "
            ) {
                "%02X".format(it)
            }
        )

        val packet = MessagePacket()


        packet.type =
            when (input.readByte().toInt()) {

                1 -> MessageType.HandshakeInit
                2 -> MessageType.HandshakeReply
                3 -> MessageType.Chat

                else ->
                    throw Exception(
                        "Unknown packet type"
                    )
            }


        packet.sessionId =
            bytesToUuid(
                readBytes(input, 16)
            )


        packet.messageId =
            bytesToUuid(
                readBytes(input, 16)
            )


        packet.timestamp =
            input.readLong()



        val nonceSize =
            input.readInt()

        packet.nonce =
            readBytes(
                input,
                nonceSize
            )



        val tagSize =
            input.readInt()

        packet.tag =
            readBytes(
                input,
                tagSize
            )



        val payloadSize =
            input.readInt()

        packet.payload =
            readBytes(
                input,
                payloadSize
            )


        return packet
    }



    private fun readBytes(
        input: DataInputStream,
        size: Int
    ): ByteArray {

        if (size < 0 || size > 10_000_000) {
            throw IllegalArgumentException(
                "Invalid packet size: $size"
            )
        }

        val buffer = ByteArray(size)

        input.readFully(buffer)

        return buffer
    }



    private fun uuidToBytes(
        uuid: UUID
    ): ByteArray {

        val bytes =
            ByteArray(16)

        val buffer =
            java.nio.ByteBuffer.wrap(bytes)


        buffer.putLong(
            uuid.mostSignificantBits
        )

        buffer.putLong(
            uuid.leastSignificantBits
        )


        return bytes
    }



    private fun bytesToUuid(
        bytes: ByteArray
    ): UUID {

        val buffer =
            java.nio.ByteBuffer.wrap(bytes)


        return UUID(
            buffer.long,
            buffer.long
        )
    }

    fun serializeHandshake(
        packet: HandshakePacket
    ): ByteArray {

        val output =
            ByteArrayOutputStream()

        val writer =
            DataOutputStream(output)


        writer.writeByte(
            packet.type.ordinal + 1
        )


        writer.writeInt(
            packet.publicKey.size
        )


        writer.write(
            packet.publicKey
        )


        writer.flush()


        return output.toByteArray()
    }
}