package com.offlinemessenger.android.protocol

import java.util.UUID

data class AckPacket(
    val sessionId: UUID,
    val messageId: UUID,
    val received: Boolean
)