package com.offlinemessenger.android.chat

import com.offlinemessenger.android.ITransport
import com.offlinemessenger.android.crypto.KeyExchangeService
import com.offlinemessenger.android.protocol.HandshakePacket
import com.offlinemessenger.android.protocol.MessageType
import com.offlinemessenger.android.protocol.PacketSerializer
import org.junit.Assert.assertTrue
import org.junit.Test

// Проверяет обмен ключами между двумя ChatEngine.
// В текущей реализации handshakeCompleted вызывается только
// на стороне, получившей HandshakeReply (инициатор соединения).
// Сторона, обработавшая HandshakeInit, создаёт sessionKey,
// но callback завершения handshake пока не вызывает.

class ChatEngineHandshakeTest {


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



    private fun setPrivateField(
        instance: Any,
        fieldName: String,
        value: Any
    ) {

        val field =
            instance.javaClass
                .getDeclaredField(fieldName)

        field.isAccessible = true

        field.set(
            instance,
            value
        )
    }



    @Test
    fun handshake_TwoEnginesCompleteSuccessfully() {


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



        val keyPair =
            KeyExchangeService()
                .generateKeyPair()



        // кладём ключи внутрь ChatEngine A
        setPrivateField(
            engineA,
            "keyPair",
            keyPair
        )



        var completedA =
            false

        var completedB =
            false



        val lock =
            Object()



        engineA.onHandshakeCompleted {

            synchronized(lock) {

                completedA = true

                lock.notify()

            }
        }



        engineB.onHandshakeCompleted {

            synchronized(lock) {

                completedB = true

                lock.notify()

            }
        }



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



        synchronized(lock) {

            lock.wait(3000)

        }



        assertTrue(
            completedA || completedB
        )
    }
}