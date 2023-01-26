package nl.kaouch.jaouad.comakership

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.TeamMember

class RecyclerAdapterTeamMembers(private val context: Context, private val members: List<TeamMember?>, private val listener: onTeamMemberClickListener): RecyclerView.Adapter<RecyclerAdapterTeamMembers.ViewHolder>() {

    private lateinit var tokenManager: TokenManager

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): RecyclerAdapterTeamMembers.ViewHolder {
        val teamView = LayoutInflater.from(context).inflate(R.layout.card_layout_specificteam_member, parent, false)

        tokenManager = TokenManager(context)
        return ViewHolder(teamView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterTeamMembers.ViewHolder, position: Int) {
        holder.memberName.text = members[position]!!.name
        holder.memberEmail.text = members[position]!!.email

    }

    override fun getItemCount(): Int {
        if (members != null) {
            if (!members.isNullOrEmpty()) {
                return members.size
            }
        }
        return -1
    }

    inner class ViewHolder(teamView: View): RecyclerView.ViewHolder(teamView), View.OnClickListener {

        var memberName: TextView = itemView.findViewById(R.id.member_name)
        var memberEmail: TextView = itemView.findViewById(R.id.member_email)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if(position != RecyclerView.NO_POSITION)
                listener.onTeamMemberClick(position)
        }
    }

    interface onTeamMemberClickListener {
        fun onTeamMemberClick(position: Int)
    }
}