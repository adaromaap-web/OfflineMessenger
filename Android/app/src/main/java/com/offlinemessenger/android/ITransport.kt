package com.offlinemessenger.android

interface ITransport {

    fun send(data: ByteArray)

    fun setReceiver(
        receiver: (ByteArray) -> Unit
    )
}