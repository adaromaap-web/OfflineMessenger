package com.offlinemessenger.android

import java.util.UUID

data class ChatItem(
    val id: UUID,
    val text: String,
    val isMine: Boolean,
    var status: String = ""
)