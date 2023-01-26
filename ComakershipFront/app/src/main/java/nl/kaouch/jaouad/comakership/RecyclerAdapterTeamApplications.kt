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
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.requests.PutComakership
import nl.kaouch.jaouad.comakership.models.responses.ComakershipApplications
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class RecyclerAdapterTeamApplications(private val putComakership: PutComakership, private val context: Context, private val applications: List<ComakershipApplications>?, private val listener: onApplicationClickListener): RecyclerView.Adapter<RecyclerAdapterTeamApplications.ViewHolder>() {

    private lateinit var tokenManager: TokenManager

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): RecyclerAdapterTeamApplications.ViewHolder {
        val applicationView = LayoutInflater.from(context).inflate(R.layout.card_layout_team_application, parent, false)
        tokenManager = TokenManager(context)
        return ViewHolder(applicationView)
    }

    override fun onBindViewHolder(holder: RecyclerAdapterTeamApplications.ViewHolder, position: Int) {
        if (!applications!!.isNullOrEmpty()) {
            holder.itemTitle.text = applications[position].team.name
            holder.itemDescription.text = applications[position].team.description

            holder.acceptBtn.setOnClickListener {
                val call = fetchApi().acceptApplicationOfTeam(
                    "Bearer " + tokenManager.getToken(),
                    applications[position].teamId,
                    putComakership.id
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

                            val call = fetchApi().updateComakershipStatus(
                                "Bearer " + tokenManager.getToken(),
                                putComakership.id,
                                PutComakership(putComakership.id, putComakership.name, putComakership.description, putComakership.credits, putComakership.bonus, 2)
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

                                        Toast.makeText(it.context, "The application is accepted!", Toast.LENGTH_SHORT)
                                            .show()
                                        val intent = Intent(context, CompanyInboxDashboardActivity::class.java)
                                        context.startActivity(intent)
                                    }
                                }

                                override fun onFailure(call: Call<Void>, t: Throwable) {
                                    Log.e("HTTP", "Could not fetch data", t)
                                    Toast.makeText(it.context, "Check the internet connection!", Toast.LENGTH_SHORT)
                                        .show()
                                }
                            })

                            Toast.makeText(it.context, "The application is accepted!", Toast.LENGTH_SHORT)
                                .show()
                            val intent = Intent(context, CompanyInboxDashboardActivity::class.java)
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

            holder.rejectBtn.setOnClickListener {
                val call = fetchApi().rejectApplicationOfTeam(
                    "Bearer " + tokenManager.getToken(),
                    applications[position].teamId,
                    putComakership.id
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
                            Toast.makeText(it.context, "The application is rejected!", Toast.LENGTH_SHORT)
                                .show()
                            val intent = Intent(context, CompanyInboxDashboardActivity::class.java)
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

        } else {
            holder.itemTitle.text = "No existing comakerships found!!"
        }
    }

    override fun getItemCount(): Int {
        if (applications != null) {
            if (!applications.isNullOrEmpty()) {
                return applications.size
            }
        }
        return -1
    }

    inner class ViewHolder(articleView: View): RecyclerView.ViewHolder(articleView), View.OnClickListener {

        var itemTitle: TextView = itemView.findViewById(R.id.team_name)
        var itemDescription: TextView = itemView.findViewById(R.id.team_description)
        var acceptBtn: Button = itemView.findViewById(R.id.accept_joinrequest_btn)
        var rejectBtn: Button = itemView.findViewById(R.id.cancel_joinrequest_btn)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if(position != RecyclerView.NO_POSITION)
                listener.onApplicationClick(position)
        }
    }

    interface onApplicationClickListener {
        fun onApplicationClick(position: Int)
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
