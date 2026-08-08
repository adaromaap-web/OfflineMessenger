package com.offlinemessenger.android

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.view.Gravity

class ChatAdapter(
    private val items: MutableList<ChatItem>
) : BaseAdapter() {

    override fun getCount(): Int = items.size

    override fun getItem(position: Int): ChatItem =
        items[position]

    override fun getItemId(position: Int): Long =
        position.toLong()

    override fun getView(
        position: Int,
        convertView: View?,
        parent: ViewGroup
    ): View {

        val view = convertView
            ?: LayoutInflater.from(parent.context)
                .inflate(
                    R.layout.item_chat,
                    parent,
                    false
                )

        val message = getItem(position)

        val textView =
            view.findViewById<TextView>(R.id.chatMessage)

        textView.text =
            if (message.status.isNotBlank()) {
                "${message.text}\n${message.status}"
            } else {
                message.text
            }

        textView.background =
            if (message.isMine) {
                parent.context.getDrawable(
                    R.drawable.chat_bubble_mine
                )
            } else {
                parent.context.getDrawable(
                    R.drawable.chat_bubble_other
                )
            }

        textView.setTextColor(Color.BLACK)

        textView.gravity =
            if (message.isMine) {
                Gravity.END
            } else {
                Gravity.START
            }

        val params =
            textView.layoutParams
                    as android.widget.FrameLayout.LayoutParams

        params.gravity =
            if (message.isMine) {
                Gravity.END
            } else {
                Gravity.START
            }

        params.width =
            ViewGroup.LayoutParams.WRAP_CONTENT

        params.height =
            ViewGroup.LayoutParams.WRAP_CONTENT

        textView.layoutParams = params

        return view
    }

    public fun add(item: ChatItem) {
        items.add(item)
        notifyDataSetChanged()
    }
}