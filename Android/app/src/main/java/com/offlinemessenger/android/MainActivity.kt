package com.offlinemessenger.android

import android.bluetooth.BluetoothAdapter
import android.bluetooth.BluetoothServerSocket
import android.bluetooth.BluetoothSocket
import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import java.io.InputStream
import java.io.OutputStream
import java.util.UUID
import kotlin.concurrent.thread
import android.Manifest

import android.content.pm.PackageManager
import androidx.core.app.ActivityCompat
import android.os.Build
import android.widget.Toast
import android.app.Activity
import com.offlinemessenger.android.chat.ChatEngine
class MainActivity : Activity() {

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

    private val SERVICE_UUID: UUID =
        UUID.fromString("00001101-0000-1000-8000-00805F9B34FB")

    private var serverSocket: BluetoothServerSocket? = null
    private var socket: BluetoothSocket? = null

    private var transport: BluetoothTransport? = null

    private var chatEngine: ChatEngine? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        if (!hasBluetoothPermission()) {
            requestBluetoothPermission()
            return
        }

        startServer()
    }

    private fun startServer() {
        val adapter = BluetoothAdapter.getDefaultAdapter()

        if (adapter == null) {
            Log.e("BT", "Bluetooth not supported")
            return
        }

        if (!adapter.isEnabled) {
            Log.e("BT", "Bluetooth is disabled")
            return
        }

        thread {
            try {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
                    if (ActivityCompat.checkSelfPermission(
                            this,
                            android.Manifest.permission.BLUETOOTH_CONNECT
                        ) != PackageManager.PERMISSION_GRANTED
                    ) {
                        Log.e("BT", "BLUETOOTH_CONNECT not granted")
                        return@thread
                    }
                }
                serverSocket =
                    adapter.listenUsingRfcommWithServiceRecord(
                        "OfflineMessenger",
                        SERVICE_UUID
                    )

                Log.d("BT", "Waiting for connection...")

                runOnUiThread {
                    Toast.makeText(this, "Bluetooth server started", Toast.LENGTH_SHORT).show()
                }

                socket = serverSocket?.accept()

                Log.d("BT", "Client connected!")


                transport = BluetoothTransport(
                    socket!!.inputStream,
                    socket!!.outputStream
                )

                Log.d(
                    "CHAT",
                    "CREATING CHAT ENGINE ${System.identityHashCode(this)}"
                )
                chatEngine = ChatEngine(
                    transport!!
                )


                chatEngine!!.onMessageReceived { message ->

                    Log.d(
                        "CHAT",
                        "CALLBACK INSTANCE hash=${System.identityHashCode(this)}"
                    )

                    Log.d(
                        "CHAT",
                        "UI received message: $message"
                    )
                }

               /** chatEngine!!.sendMessage(
                    "HELLO FROM ANDROID CHAT ENGINE"
                ) */
                    //listen(socket!!.inputStream, socket!!.outputStream)

            } catch (e: Exception) {
                Log.e("BT", "Error: ${e.message}")
            }
        }
    }
}