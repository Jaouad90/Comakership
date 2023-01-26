package nl.kaouch.jaouad.comakership

import android.content.Context
import android.content.Intent
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.responses.TeamJoinRequest
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class RecyclerAdapterJoinRequests(
    private val context: Context,
    private val joinRequests: List<TeamJoinRequest>?,
    private val listener: onInboxClickListener
): RecyclerView.Adapter<RecyclerAdapterJoinRequests.ViewHolder>() {

    private lateinit var tokenManager: TokenManager

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): RecyclerAdapterJoinRequests.ViewHolder {
        val teamView = LayoutInflater.from(context).inflate(
            R.layout.card_layout_joined_teams,
            parent,
            false
        )

        tokenManager = TokenManager(context)
        return ViewHolder(teamView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterJoinRequests.ViewHolder, position: Int) {
        holder.teamName.text = joinRequests!![position].team.name
        holder.studentName.text = joinRequests[position].studentUser.name
        holder.acceptBtn.setOnClickListener {
            val call = fetchApi().acceptJoinRequest(
                "Bearer " + tokenManager.getToken(),
                joinRequests[position].teamId,
                joinRequests[position].studentUserId
            )
            call.enqueue(object : Callback<Void> {

                override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
                ) {
                    if(response.code() == 401) {
                        tokenManager.clearJwtToken()
                        val intent = Intent(context, LoginActivity::class.java)
                        context.startActivity(intent)
                    }
                    if (response.isSuccessful) {
                        val intent = Intent(context, InboxActivity::class.java)
                        context.startActivity(intent)
                    }
                }

                override fun onFailure(call: Call<Void>, t: Throwable) {
                    Log.e("HTTP", "Could not fetch data", t)
                    Toast.makeText(it.context, "Check the internet connection!", Toast.LENGTH_SHORT)
                        .show()
                }
            })
        }

        holder.cancelBtn.setOnClickListener {
            val call = fetchApi().rejectJoinRequest(
                "Bearer " + tokenManager.getToken(),
                joinRequests[position].teamId,
                joinRequests[position].studentUserId
            )
            call.enqueue(object : Callback<Void> {

                override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
                ) {
                    if(response.code() == 401) {
                        tokenManager.clearJwtToken()
                        val intent = Intent(context, LoginActivity::class.java)
                        context.startActivity(intent)
                    }
                    if (response.isSuccessful) {
                        val intent = Intent(context, InboxActivity::class.java)
                        context.startActivity(intent)
                    }
                }

                override fun onFailure(call: Call<Void>, t: Throwable) {
                    Log.e("HTTP", "Could not fetch data", t)
                    Toast.makeText(it.context, "Check the internet connection!", Toast.LENGTH_SHORT)
                        .show()
                }
            })
        }
    }

    override fun getItemCount(): Int {
        if (joinRequests != null) {
            if (!joinRequests.isNullOrEmpty()) {
                return joinRequests.size
            }
        }
        return -1
    }

    inner class ViewHolder(teamView: View): RecyclerView.ViewHolder(teamView), View.OnClickListener {

        var teamName: TextView = itemView.findViewById(R.id.member_name)
        var studentName: TextView = itemView.findViewById(R.id.member_email)
        var acceptBtn: Button = itemView.findViewById(R.id.accept_joinrequest_btn)
        var cancelBtn: Button = itemView.findViewById(R.id.cancel_joinrequest_btn)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if(position != RecyclerView.NO_POSITION)
                listener.onInboxClick(position)
        }
    }

    interface onInboxClickListener {
        fun onInboxClick(position: Int)
    }

    fun fetchApi(): ApiInterface {
        val retrofitBuilder = Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ApiInterface::class.java)

        val service: ApiInterface = retrofitBuilder
        return service
    }
}