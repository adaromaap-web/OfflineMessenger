package com.offlinemessenger.android.transport

import com.offlinemessenger.android.ITransport
import org.junit.Test
import org.junit.Assert.assertArrayEquals
import org.junit.Assert.assertTrue


class BluetoothTransportTest {


    @Test
    fun transport_SendData_DeliversToReceiver() {

        // Проверяет контракт транспортного слоя.
        // Отправленные данные должны попасть в receiver.
        // Реальный Bluetooth будет проверяться отдельно
        // после появления второго клиента (Windows).


        val transport =
            FakeTransport()


        val expected =
            "Hello Bluetooth".toByteArray()


        var received: ByteArray? =
            null



        transport.setReceiver { data ->

            received =
                data
        }



        transport.send(
            expected
        )



        assertTrue(
            received != null
        )


        assertArrayEquals(
            expected,
            received
        )
    }
}



class FakeTransport : ITransport {


    private var receiver:
            ((ByteArray) -> Unit)? = null



    override fun send(
        data: ByteArray
    ) {

        receiver?.invoke(
            data
        )
    }



    override fun setReceiver(
        receiver: (ByteArray) -> Unit
    ) {

        this.receiver =
            receiver
    }
}