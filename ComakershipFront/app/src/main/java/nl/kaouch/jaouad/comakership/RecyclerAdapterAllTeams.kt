package nl.kaouch.jaouad.comakership

import android.content.Context
import android.content.Intent
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.PrivateTeam
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class RecyclerAdapterAllTeams(private val context: Context, private val teams: List<PrivateTeam?>, private val listener: onTeamClickListener): RecyclerView.Adapter<RecyclerAdapterAllTeams.ViewHolder>() {

    private lateinit var tokenManager: TokenManager

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): RecyclerAdapterAllTeams.ViewHolder {
        val teamView = LayoutInflater.from(context).inflate(R.layout.card_layout_all_teams, parent, false)

        tokenManager = TokenManager(context)
        return ViewHolder(teamView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterAllTeams.ViewHolder, position: Int) {
        holder.teamName.text = teams!![position]!!.name
        holder.teamDescription.text = teams[position]!!.description
        holder.joinBtn.setOnClickListener {
            val call = fetchApi().joinTeam("Bearer " + tokenManager.getToken(), teams[position]!!.id)
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
                        Toast.makeText(it.context, "Succesfully joined team "+ teams!![position]!!.name+"!!", Toast.LENGTH_SHORT).show()
                    }
                    else {
                        Toast.makeText(it.context, "You've already send in a request!! "+ teams!![position]!!.name+"!!", Toast.LENGTH_SHORT).show()
                    }
                }

                override fun onFailure(call: Call<Void>, t: Throwable) {
                    Log.e("HTTP", "Could not fetch data", t)
                }
            })
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
        var teamDescription: TextView = itemView.findViewById(R.id.team_description_value)
        var joinBtn: ImageView = itemView.findViewById(R.id.join_team_btn)

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