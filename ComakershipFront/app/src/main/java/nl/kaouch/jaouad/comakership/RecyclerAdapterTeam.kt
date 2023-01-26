package nl.kaouch.jaouad.comakership

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.models.responses.SpecificTeam

class RecyclerAdapterTeam(private val context: Context, private val teams: List<SpecificTeam?>, private val listener: onTeamClickListener): RecyclerView.Adapter<RecyclerAdapterTeam.ViewHolder>() {
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerAdapterTeam.ViewHolder {
        val teamView = LayoutInflater.from(context).inflate(R.layout.card_layout_my_teams, parent, false)
        return ViewHolder(teamView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterTeam.ViewHolder, position: Int) {
        holder.teamName.text = teams[position]!!.name
        if(teams[position]!!.members != null) {
            holder.teamSize.text = teams[position]!!.members.size.toString()
        } else {
            holder.teamSize.text = 0.toString()
        }
    }

    override fun getItemCount(): Int {
        if (teams != null) {
            if (!teams.isNullOrEmpty()) {
                return teams.size
            }
        }
        return -1
    }

    inner class ViewHolder(teamView: View): RecyclerView.ViewHolder(teamView), View.OnClickListener {

        var teamName: TextView = itemView.findViewById(R.id.member_name)
        var teamSize: TextView = itemView.findViewById(R.id.team_description_value)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if(position != RecyclerView.NO_POSITION)
                listener.onTeamClick(position)
        }
    }

    interface onTeamClickListener {
        fun onTeamClick(position: Int)
    }
}