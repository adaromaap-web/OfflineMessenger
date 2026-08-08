package com.offlinemessenger.android

import java.util.UUID

data class StoredMessage(
    val id: UUID,
    val text: String,
    val isMine: Boolean,
    val timestamp: Long,
    val status: String
)