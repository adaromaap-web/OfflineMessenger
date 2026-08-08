
package com.offlinemessenger.android

import android.Manifest
import android.app.Activity
import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothServerSocket
import android.bluetooth.BluetoothSocket
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.ListView
import android.widget.Toast
import androidx.core.app.ActivityCompat
import com.offlinemessenger.android.chat.ChatEngine
import java.util.UUID
import kotlin.concurrent.thread

class MainActivity : Activity() {

    private lateinit var chatList: ListView
    private lateinit var messageInput: EditText
    private lateinit var sendButton: Button

    private lateinit var adapter: ChatAdapter

    private val messages = mutableListOf<ChatItem>()

    private lateinit var messageRepository: MessageRepository

    private val SERVICE_UUID: UUID =
        UUID.fromString(
            "00001101-0000-1000-8000-00805F9B34FB"
        )

    private var serverSocket: BluetoothServerSocket? = null
    private var socket: BluetoothSocket? = null

    private var transport: BluetoothTransport? = null

    private var chatEngine: ChatEngine? = null


    private fun hasBluetoothPermission(): Boolean {

        return ActivityCompat.checkSelfPermission(
            this,
            Manifest.permission.BLUETOOTH_CONNECT
        ) == PackageManager.PERMISSION_GRANTED
    }


    private fun requestBluetoothPermission() {

        ActivityCompat.requestPermissions(
            this,
            arrayOf(
                Manifest.permission.BLUETOOTH_CONNECT,
                Manifest.permission.BLUETOOTH_SCAN
            ),
            1
        )
    }


    override fun onCreate(
        savedInstanceState: Bundle?
    ) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_main)


        messageRepository =
            MessageRepository(
                MessageDatabase(this)
            )


        chatList =
            findViewById(R.id.chatList)

        messageInput =
            findViewById(R.id.messageInput)

        sendButton =
            findViewById(R.id.sendButton)


        adapter =
            ChatAdapter(messages)

        chatList.adapter =
            adapter


        loadMessages()


        if (!hasBluetoothPermission()) {

            requestBluetoothPermission()

            return
        }


        startServer()
    }


    private fun loadMessages() {

        val savedMessages =
            messageRepository.getAll()


        messages.clear()


        messages.addAll(
            savedMessages.map { stored ->

                ChatItem(
                    id = stored.id,
                    text = stored.text,
                    isMine = stored.isMine,
                    status = stored.status
                )
            }
        )


        adapter.notifyDataSetChanged()


        Log.d(
            "DB",
            "Loaded messages: ${messages.size}"
        )
    }


    private fun startServer() {

        val bluetoothAdapter =
            BluetoothAdapter.getDefaultAdapter()


        if (bluetoothAdapter == null) {

            Log.e(
                "BT",
                "Bluetooth not supported"
            )

            return
        }


        if (!bluetoothAdapter.isEnabled) {

            Log.e(
                "BT",
                "Bluetooth is disabled"
            )

            return
        }


        thread {

            try {

                if (
                    Build.VERSION.SDK_INT >=
                    Build.VERSION_CODES.S
                ) {

                    if (
                        ActivityCompat.checkSelfPermission(
                            this,
                            Manifest.permission.BLUETOOTH_CONNECT
                        ) != PackageManager.PERMISSION_GRANTED
                    ) {

                        Log.e(
                            "BT",
                            "BLUETOOTH_CONNECT not granted"
                        )

                        return@thread
                    }
                }


                serverSocket =
                    bluetoothAdapter
                        .listenUsingRfcommWithServiceRecord(
                            "OfflineMessenger",
                            SERVICE_UUID
                        )


                Log.d(
                    "BT",
                    "Waiting for connection..."
                )


                runOnUiThread {

                    Toast.makeText(
                        this,
                        "Bluetooth server started",
                        Toast.LENGTH_SHORT
                    ).show()
                }


                socket =
                    serverSocket?.accept()


                Log.d(
                    "BT",
                    "Client connected!"
                )


                transport =
                    BluetoothTransport(
                        socket!!.inputStream,
                        socket!!.outputStream
                    )


                chatEngine =
                    ChatEngine(
                        transport!!
                    )


                chatEngine!!.onHandshakeCompleted {

                    Log.d(
                        "CHAT",
                        "HANDSHAKE READY"
                    )
                }


                chatEngine!!.onMessageReceived {
                        messageId,
                        message ->

                    Log.d(
                        "CHAT",
                        "UI received message: $message"
                    )


                    val storedMessage =
                        StoredMessage(
                            id = messageId,
                            text = message,
                            isMine = false,
                            timestamp =
                                System.currentTimeMillis(),
                            status = ""
                        )


                    messageRepository.add(
                        storedMessage
                    )


                    runOnUiThread {

                        messages.add(
                            ChatItem(
                                id = messageId,
                                text = message,
                                isMine = false,
                                status = ""
                            )
                        )


                        adapter.notifyDataSetChanged()


                        chatList.setSelection(
                            adapter.count - 1
                        )
                    }
                }


                runOnUiThread {

                    sendButton.setOnClickListener {

                        sendMessage()
                    }
                }


                chatEngine!!.onHandshakeCompleted {

                    Log.d(
                        "CHAT",
                        "Handshake completed"
                    )
                }


            } catch (e: Exception) {

                Log.e(
                    "BT",
                    "Error: ${e.message}",
                    e
                )
            }
        }
    }


    private fun sendMessage() {

        val text =
            messageInput.text
                .toString()


        if (text.isBlank()) {
            return
        }


        val engine =
            chatEngine


        if (engine == null) {

            Toast.makeText(
                this,
                "Сообщение не отправлено: нет соединения",
                Toast.LENGTH_SHORT
            ).show()

            return
        }


        val messageId =
            engine.sendMessage(text)


        if (messageId == null) {

            Toast.makeText(
                this,
                "Сообщение не отправлено",
                Toast.LENGTH_SHORT
            ).show()

            return
        }


        val timestamp =
            System.currentTimeMillis()


        val item =
            ChatItem(
                id = messageId,
                text = text,
                isMine = true,
                status = ""
            )


        messages.add(item)


        adapter.notifyDataSetChanged()


        chatList.setSelection(
            adapter.count - 1
        )


        messageRepository.add(
            StoredMessage(
                id = messageId,
                text = text,
                isMine = true,
                timestamp = timestamp,
                status = "⏳ Sending"
            )
        )


        messageInput.text.clear()
    }
}

