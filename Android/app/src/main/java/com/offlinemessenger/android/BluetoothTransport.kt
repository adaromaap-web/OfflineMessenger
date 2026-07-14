package com.offlinemessenger.android

import java.io.DataInputStream
import java.io.InputStream
import java.io.OutputStream
import java.nio.ByteBuffer


class BluetoothTransport(
    private val input: InputStream,
    private val output: OutputStream
) : ITransport {


    private var receiver:
            ((ByteArray) -> Unit)? = null



    override fun send(
        data: ByteArray
    ) {

        val size =
            ByteBuffer
                .allocate(4)
                .putInt(data.size)
                .array()


        output.write(size)
        output.write(data)

        output.flush()
    }



    override fun setReceiver(
        receiver: (ByteArray) -> Unit
    ) {

        this.receiver = receiver

        startReceiving()
    }



    private fun startReceiving() {

        Thread {

            val reader =
                DataInputStream(input)


            while (true) {

                try {

                    // читаем длину
                    val size =
                        reader.readInt()


                    if (size <= 0 || size > 10_000_000) {
                        continue
                    }


                    val data =
                        ByteArray(size)


                    reader.readFully(data)


                    receiver?.invoke(data)


                } catch (e: Exception) {

                    break
                }
            }

        }.start()
    }
}