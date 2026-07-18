package com.offlinemessenger.android.chat

import android.util.Log
import com.offlinemessenger.android.ITransport
import com.offlinemessenger.android.crypto.KeyExchangeService
import com.offlinemessenger.android.crypto.KeyPair
import com.offlinemessenger.android.crypto.Hkdf
import com.offlinemessenger.android.crypto.CryptoService
import com.offlinemessenger.android.protocol.MessageType
import com.offlinemessenger.android.protocol.PacketSerializer
import com.offlinemessenger.android.protocol.HandshakePacket
import java.io.ByteArrayInputStream
import java.io.DataInputStream
import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.util.UUID
import com.offlinemessenger.android.protocol.AckPacket

class ChatEngine(
    private val transport: ITransport
) {

    private val keyExchange = KeyExchangeService()

    private var keyPair: KeyPair? = null

    private var sharedSecret: ByteArray? = null

    private var sessionKey: ByteArray? = null

    private var messageReceiver: ((String) -> Unit)? = null


    init {

        transport.setReceiver { data ->

            Log.d(
                "CHAT",
                "Receiver got packet hash=${data.contentHashCode()}"
            )

            try {

                if (data.isEmpty()) {
                    return@setReceiver
                }


                Log.d(
                    "CHAT",
                    "Packet type=${data[0].toInt()}, size=${data.size}"
                )


                when (data[0].toInt()) {


                    1 -> handleHandshakeInit(data)


                    2 -> handleHandshakeReply(data)


                    3 -> {

                        val packet =
                            PacketSerializer.deserialize(data)


                        val decrypted =
                            CryptoService().decrypt(
                                sessionKey!!,
                                packet.payload,
                                packet.nonce,
                                packet.tag
                            )


                        val message =
                            String(decrypted)


                        Log.d(
                            "CHAT",
                            "ChatEngine decrypted message: $message"
                        )

                        sendAck(
                            packet.sessionId,
                            packet.messageId
                        )

                        Log.d(
                            "BT",
                            "DELIVER ONCE hash=${data.hashCode()}"
                        )

                        Log.d(
                            "CHAT",
                            "INVOKING RECEIVER hash=${message.hashCode()}"
                        )

                        Log.d(
                            "CHAT",
                            "BEFORE MESSAGE RECEIVER hash=${message.hashCode()}"
                        )

                        messageReceiver?.invoke(message)
                    }


                    else -> {

                        Log.e(
                            "CHAT",
                            "Unknown packet type"
                        )
                    }
                }


            } catch (e: Exception) {

                Log.e(
                    "CHAT",
                    "Packet error: ${e.message}"
                )
            }
        }
    }



    private fun handleHandshakeInit(
        data: ByteArray
    ) {


        Log.d(
            "CHAT",
            "HandshakeInit received"
        )


        val remoteKey =
            readPublicKey(data)



        keyPair =
            keyExchange.generateKeyPair()



        Log.d(
            "CHAT",
            "ANDROID LOCAL PUB: ${
                keyPair!!.publicKey.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )



        sharedSecret =
            keyExchange.deriveSharedSecret(
                keyPair!!.privateKey,
                remoteKey
            )



        Log.d(
            "CHAT",
            "ANDROID SHARED RAW: ${
                sharedSecret!!.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )



        sessionKey =
            Hkdf.deriveKey(
                sharedSecret!!
            )



        Log.d(
            "CHAT",
            "ANDROID SESSION KEY: ${
                sessionKey!!.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )



        val reply =
            HandshakePacket()


        reply.type =
            MessageType.HandshakeReply


        reply.publicKey =
            keyPair!!.publicKey



        transport.send(
            PacketSerializer.serializeHandshake(reply)
        )



        Log.d(
            "CHAT",
            "HandshakeReply sent"
        )
    }





    private fun handleHandshakeReply(
        data: ByteArray
    ) {


        Log.d(
            "CHAT",
            "HandshakeReply received"
        )



        val remoteKey =
            readPublicKey(data)



        if (keyPair == null) {

            Log.e(
                "CHAT",
                "Missing local key pair"
            )

            return
        }



        sharedSecret =
            keyExchange.deriveSharedSecret(
                keyPair!!.privateKey,
                remoteKey
            )



        Log.d(
            "CHAT",
            "ANDROID SHARED RAW: ${
                sharedSecret!!.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )



        sessionKey =
            Hkdf.deriveKey(
                sharedSecret!!
            )



        Log.d(
            "CHAT",
            "ANDROID SESSION KEY: ${
                sessionKey!!.joinToString("") {
                    "%02X".format(it)
                }
            }"
        )
    }





    private fun readPublicKey(
        data: ByteArray
    ): ByteArray {


        val input =
            DataInputStream(
                ByteArrayInputStream(data)
            )


        // type
        input.readByte()



        val sizeBytes =
            ByteArray(4)


        input.readFully(sizeBytes)



        val size =
            ByteBuffer
                .wrap(sizeBytes)
                .order(ByteOrder.LITTLE_ENDIAN)
                .int



        if (size <= 0 || size > 10000) {

            throw Exception(
                "Invalid public key size: $size"
            )
        }



        val key =
            ByteArray(size)



        input.readFully(key)



        return key
    }





    fun sendMessage(
        message: String
    ) {

        transport.send(
            message.toByteArray()
        )
    }





    fun onMessageReceived(
        receiver: (String) -> Unit
    ) {

        Log.d(
            "CHAT",
            "REGISTER RECEIVER"
        )

        messageReceiver = receiver
    }

    private fun sendAck(
        sessionId: UUID,
        messageId: UUID
    ) {

        val ack =
            AckPacket(
                sessionId = sessionId,
                messageId = messageId,
                received = true
            )


        transport.send(
            PacketSerializer.serializeAck(ack)
        )


        Log.d(
            "CHAT",
            "ACK sent: $messageId"
        )
    }
}