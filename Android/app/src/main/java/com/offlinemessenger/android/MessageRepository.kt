package com.offlinemessenger.android

import android.content.ContentValues
import java.util.UUID

class MessageRepository(
    private val database: MessageDatabase
) {

    fun add(message: StoredMessage) {
        val db = database.writableDatabase

        val values = ContentValues().apply {
            put("Id", message.id.toString())
            put("Text", message.text)
            put("IsMine", if (message.isMine) 1 else 0)
            put("Timestamp", message.timestamp)
            put("Status", message.status)
        }

        db.insertOrThrow(
            "Messages",
            null,
            values
        )
    }

    fun getAll(): List<StoredMessage> {
        val db = database.readableDatabase

        val messages = mutableListOf<StoredMessage>()

        val cursor = db.query(
            "Messages",
            arrayOf(
                "Id",
                "Text",
                "IsMine",
                "Timestamp",
                "Status"
            ),
            null,
            null,
            null,
            null,
            "Timestamp ASC"
        )

        cursor.use {
            while (it.moveToNext()) {

                val message = StoredMessage(
                    id = UUID.fromString(
                        it.getString(
                            it.getColumnIndexOrThrow("Id")
                        )
                    ),
                    text = it.getString(
                        it.getColumnIndexOrThrow("Text")
                    ),
                    isMine = it.getInt(
                        it.getColumnIndexOrThrow("IsMine")
                    ) == 1,
                    timestamp = it.getLong(
                        it.getColumnIndexOrThrow("Timestamp")
                    ),
                    status = it.getString(
                        it.getColumnIndexOrThrow("Status")
                    )
                )

                messages.add(message)
            }
        }

        return messages
    }

    fun deleteAll() {
        val db = database.writableDatabase

        db.delete(
            "Messages",
            null,
            null
        )
    }
}