package com.offlinemessenger.android

import android.content.Context
import android.database.sqlite.SQLiteDatabase
import android.database.sqlite.SQLiteOpenHelper

class MessageDatabase(context: Context) :
    SQLiteOpenHelper(
        context,
        "messages.db",
        null,
        1
    ) {

    override fun onCreate(db: SQLiteDatabase) {
        db.execSQL(
            """
            CREATE TABLE Messages (
                Id TEXT NOT NULL PRIMARY KEY,
                Text TEXT NOT NULL,
                IsMine INTEGER NOT NULL,
                Timestamp INTEGER NOT NULL,
                Status TEXT NOT NULL
            )
            """.trimIndent()
        )
    }

    override fun onUpgrade(
        db: SQLiteDatabase,
        oldVersion: Int,
        newVersion: Int
    ) {
        // Пока миграций нет.
    }
}