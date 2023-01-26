package nl.kaouch.jaouad.comakership

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.models.Comakership

class RecyclerAdapterInboxComakerships(private val context: Context, private val comakerships: List<Comakership>?, private val listener: onComakershipClickListener): RecyclerView.Adapter<RecyclerAdapterInboxComakerships.ViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerAdapterInboxComakerships.ViewHolder {
        val comakershipView = LayoutInflater.from(context).inflate(R.layout.card_layout_company_comakership, parent, false)
        return ViewHolder(comakershipView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterInboxComakerships.ViewHolder, position: Int) {

        if (!comakerships!!.isNullOrEmpty()) {
            if (!comakerships[position].name.isNullOrEmpty() || !comakerships[position].createdAt.isNullOrEmpty())
                holder.itemTitle.text = comakerships[position].name
                var date = comakerships[position].createdAt.substring(0, comakerships[position].createdAt.indexOf("T"))
                holder.itemDate.text = date
        } else {
            holder.itemTitle.text = "No existing comakerships found!!"
        }
    }

    override fun getItemCount(): Int {
        if (comakerships != null) {
            if (!comakerships.isNullOrEmpty()) {
                return comakerships.size
            }
        }
        return -1
    }

    inner class ViewHolder(articleView: View): RecyclerView.ViewHolder(articleView), View.OnClickListener {

        var itemTitle: TextView = itemView.findViewById(R.id.deliverable_title)
        var itemStatus: TextView = itemView.findViewById(R.id.deliverable_name)
        var itemDate: TextView = itemView.findViewById(R.id.team_description_value)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if(position != RecyclerView.NO_POSITION)
                listener.onComakershipClick(position)
        }
    }

    interface onComakershipClickListener {
        fun onComakershipClick(position: Int)
    }
}