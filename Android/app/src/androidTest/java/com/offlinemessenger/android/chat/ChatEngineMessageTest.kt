package com.offlinemessenger.android.chat

import com.offlinemessenger.android.ITransport
import com.offlinemessenger.android.crypto.KeyExchangeService
import com.offlinemessenger.android.protocol.HandshakePacket
import com.offlinemessenger.android.protocol.MessageType
import com.offlinemessenger.android.protocol.PacketSerializer
import org.junit.Assert.assertEquals
import org.junit.Test

// Проверяет полный цикл отправки сообщения через ChatEngine.
// После установки защищённой сессии сообщение должно быть зашифровано,
// передано через транспорт, расшифровано и доставлено получателю.

class ChatEngineMessageTest {


    private class FakeTransport : ITransport {

        private var receiver:
                ((ByteArray) -> Unit)? = null


        private var remote:
                FakeTransport? = null



        override fun send(data: ByteArray) {

            remote
                ?.receiver
                ?.invoke(data)

        }



        override fun setReceiver(
            receiver: (ByteArray) -> Unit
        ) {

            this.receiver = receiver

        }



        fun connect(
            other: FakeTransport
        ) {

            remote = other

        }
    }



    @Test
    fun afterHandshake_SendMessage_MessageReceivedCalled() {


        val transportA =
            FakeTransport()


        val transportB =
            FakeTransport()



        transportA.connect(
            transportB
        )

        transportB.connect(
            transportA
        )



        val engineA =
            ChatEngine(
                transportA
            )


        val engineB =
            ChatEngine(
                transportB
            )



        val lock =
            Object()



        var handshakeCount =
            0


        var received:
                String? = null



        engineA.onHandshakeCompleted {

            synchronized(lock) {

                handshakeCount++

                lock.notify()

            }

        }



        engineB.onHandshakeCompleted {

            synchronized(lock) {

                handshakeCount++

                lock.notify()

            }

        }



        engineB.onMessageReceived { message ->

            synchronized(lock) {

                received = message

                lock.notify()

            }

        }



        // Запускаем handshake вручную

        val keyPair =
            KeyExchangeService()
                .generateKeyPair()



        val init =
            HandshakePacket()


        init.type =
            MessageType.HandshakeInit


        init.publicKey =
            keyPair.publicKey



        transportA.send(
            PacketSerializer.serializeHandshake(
                init
            )
        )



        // Ждём handshake обеих сторон

        synchronized(lock) {

            var time = 0

            while (
                handshakeCount < 2 &&
                time < 20
            ) {

                lock.wait(500)

                time++

            }

        }



        assertEquals(
            2,
            handshakeCount
        )



        engineA.sendMessage(
            "Hello Android"
        )



        // Ждём доставки сообщения

        synchronized(lock) {

            var time = 0

            while (
                received == null &&
                time < 20
            ) {

                lock.wait(500)

                time++

            }

        }



        assertEquals(
            "Hello Android",
            received
        )
    }
}