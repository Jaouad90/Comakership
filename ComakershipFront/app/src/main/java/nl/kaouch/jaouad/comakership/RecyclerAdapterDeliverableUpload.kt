package nl.kaouch.jaouad.comakership

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.cardview.widget.CardView
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.models.Deliverable


class RecyclerAdapterDeliverableUpload(
    private val context: Context,
    private val deliverables: List<Deliverable>?,
    private val listener: onDeliverableClickListener
): RecyclerView.Adapter<RecyclerAdapterDeliverableUpload.ViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerAdapterDeliverableUpload.ViewHolder {
        val comakershipView = LayoutInflater.from(context).inflate(
            R.layout.card_layout_deliverable_upload,
            parent,
            false
        )
        return ViewHolder(comakershipView)
    }

    override fun onBindViewHolder(
        holder: RecyclerAdapterDeliverableUpload.ViewHolder,
        position: Int
    ) {

        if (!deliverables!!.isNullOrEmpty()) {
            if (!deliverables[position].name.isNullOrEmpty())
                holder.title.text = deliverables[position].name
            if(deliverables[position].finished!!.equals(true)) {
                holder.cardView.setBackgroundColor(ContextCompat.getColor(context, R.color.primary_green))
            } else {
                holder.cardView.setBackgroundColor(ContextCompat.getColor(context, R.color.status_red))
            }
        } else {
            holder.title.text = "No existing comakerships found!!"
        }
    }

    override fun getItemCount(): Int {
        if (deliverables != null) {
            if (!deliverables.isNullOrEmpty()) {
                return deliverables.size
            }
        }
        return -1
    }

    inner class ViewHolder(articleView: View): RecyclerView.ViewHolder(articleView), View.OnClickListener {
        var title: TextView = itemView.findViewById(R.id.deliverable_title)
        var cardView: CardView = itemView.findViewById(R.id.deliverable_upload_card_view)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if(position != RecyclerView.NO_POSITION)
                listener.onDeliverableClick(position)
        }
    }

    interface onDeliverableClickListener {
        fun onDeliverableClick(position: Int)
    }
}