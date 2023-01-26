package nl.kaouch.jaouad.comakership

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.models.Deliverable


class RecyclerAdapterDeliverable(
    private val context: Context,
    private var deliverables: MutableList<Deliverable>?
): RecyclerView.Adapter<RecyclerAdapterDeliverable.ViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerAdapterDeliverable.ViewHolder {
        val comakershipView = LayoutInflater.from(context).inflate(
            R.layout.card_layout_deliverables_comakership_create,
            parent,
            false
        )
        return ViewHolder(comakershipView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterDeliverable.ViewHolder, position: Int) {

        if (deliverables!!.isNotEmpty()) {
            if (deliverables!![position].name.isNotEmpty())
                holder.itemTitle.text = "Deliverable"
                holder.itemName.text = deliverables!![position].name
                holder.itemRemoveBtn.setOnClickListener {
                    deliverables!!.remove(deliverables!![position])
                    notifyItemRemoved(position)
                    notifyDataSetChanged()
                }
        } else {
            holder.itemTitle.text = "No existing comakerships found!!"
        }
    }

    fun getList(): List<Deliverable?>? {

        notifyDataSetChanged()
        return deliverables!!.toMutableList()
    }

    override fun getItemCount(): Int {
        if (deliverables != null) {
            if (!deliverables.isNullOrEmpty()) {
                return deliverables!!.size
            }
        }
        return -1
    }

    inner class ViewHolder(articleView: View): RecyclerView.ViewHolder(articleView){

        var itemTitle: TextView = itemView.findViewById(R.id.deliverable_title)
        var itemName: TextView = itemView.findViewById(R.id.deliverable_name)
        var itemRemoveBtn: ImageView = itemView.findViewById(R.id.delete_deliverable_btn)

    }
}