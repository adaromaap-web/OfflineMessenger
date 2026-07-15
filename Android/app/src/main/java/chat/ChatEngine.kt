package com.offlinemessenger.android.chat

import android.util.Log
import com.offlinemessenger.android.ITransport
import com.offlinemessenger.android.crypto.KeyExchangeService
import com.offlinemessenger.android.crypto.KeyPair
import com.offlinemessenger.android.crypto.Hkdf
import com.offlinemessenger.android.protocol.MessageType
import com.offlinemessenger.android.protocol.PacketSerializer
import com.offlinemessenger.android.protocol.HandshakePacket
import java.io.ByteArrayInputStream
import java.io.DataInputStream


class ChatEngine(
    private val transport: ITransport
) {


    private val keyExchange =
        KeyExchangeService()


    private var keyPair: KeyPair? = null


    private var sharedSecret: ByteArray? = null


    private var sessionKey: ByteArray? = null



    private var messageReceiver:
            ((String) -> Unit)? = null



    init {

        transport.setReceiver { data ->

            try {


                if (data.isEmpty()) {
                    return@setReceiver
                }


                when (data[0].toInt()) {


                    1 -> {

                        handleHandshakeInit(
                            data
                        )
                    }


                    2 -> {

                        handleHandshakeReply(data)
                    }




                    3 -> {

                        val packet =
                            PacketSerializer.deserialize(
                                data
                            )


                        val message =
                            String(packet.payload)


                        Log.d(
                            "CHAT",
                            "Chat packet received"
                        )


                        messageReceiver?.invoke(
                            message
                        )
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
                "No local key pair"
            )

            return
        }


        sharedSecret =
            keyExchange.deriveSharedSecret(
                keyPair!!.privateKey,
                remoteKey
            )


        sessionKey =
            Hkdf.deriveKey(
                sharedSecret!!
            )


        Log.d(
            "CHAT",
            "Shared secret created from reply"
        )
    }

    private fun handleHandshakeInit(
        data: ByteArray
    ) {


        Log.d(
            "CHAT",
            "HandshakeInit received"
        )


        val remoteKey =
            readPublicKey(
                data
            )


        keyPair =
            keyExchange.generateKeyPair()



        sharedSecret =
            keyExchange.deriveSharedSecret(
                keyPair!!.privateKey,
                remoteKey
            )


        sessionKey =
            Hkdf.deriveKey(
                sharedSecret!!
            )



        val reply =
            HandshakePacket()


        reply.type =
            MessageType.HandshakeReply


        reply.publicKey =
            keyPair!!.publicKey



        transport.send(
            PacketSerializer.serializeHandshake(
                reply
            )
        )


        Log.d(
            "CHAT",
            "HandshakeReply sent"
        )


        Log.d(
            "CHAT",
            "Shared secret created"
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
            java.nio.ByteBuffer
                .wrap(sizeBytes)
                .order(java.nio.ByteOrder.LITTLE_ENDIAN)
                .int



        if (size < 0 || size > 10000) {
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

        messageReceiver =
            receiver
    }
}