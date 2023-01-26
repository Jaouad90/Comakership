package nl.kaouch.jaouad.comakership.student.inbox

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.PrivateTeam
import nl.kaouch.jaouad.comakership.models.responses.TeamJoinRequest
import nl.kaouch.jaouad.comakership.student.comakership.ComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import nl.kaouch.jaouad.comakership.student.team.TeamDashboardActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class InboxActivity : AppCompatActivity(), RecyclerAdapterJoinRequests.onInboxClickListener {

    private lateinit var recyclerview_all_teams: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterJoinRequests
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var joinRequestsOfTeam: List<TeamJoinRequest>
    private lateinit var logout_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_inbox)

        tokenManager = TokenManager(applicationContext)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_all_teams = findViewById(R.id.recyclerview_student_joinrequests)
        joinRequestsOfTeam = emptyList()
        logout_button = findViewById(R.id.logout_button)

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@InboxActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent)
        }

        recyclerview_all_teams.setHasFixedSize(true)
        recyclerview_all_teams.layoutManager = LinearLayoutManager(this)

        getTeams(tokenManager.getToken())

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_inbox
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@InboxActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@InboxActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@InboxActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@InboxActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@InboxActivity,
                        ComakershipDashboardActivity::class.java
                    )
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }

    private fun getTeams(token: String) {
        val call = fetchApi().getTeams("Bearer "+token)
        call.enqueue(object : Callback<List<PrivateTeam>> {

            override fun onResponse(
                call: Call<List<PrivateTeam>>,
                response: Response<List<PrivateTeam>>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@InboxActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    var teamList = response.body()
                    teamList!!.forEach {

                        val call = fetchApi().getTeamJoinRequests("Bearer "+token, it.id)
                        call.enqueue(object : Callback<List<TeamJoinRequest>> {

                            override fun onResponse(
                                call: Call<List<TeamJoinRequest>>,
                                response: Response<List<TeamJoinRequest>>
                            ) {
                                if(response.code() == 401) {
                                    tokenManager.clearJwtToken()
                                    val intent = Intent(this@InboxActivity, LoginActivity::class.java)
                                    finish()
                                    startActivity(intent)
                                }
                                if (response.isSuccessful) {
                                    if(response.code() == 200) {
                                        joinRequestsOfTeam += response.body()!!.toMutableList()
                                        createJoinRequestRecyclerAdapter()
                                    }
                                }
                            }

                            override fun onFailure(call: Call<List<TeamJoinRequest>>, t: Throwable) {
                                Log.e("HTTP", "Could not fetch data", t)
                                Toast.makeText(this@InboxActivity, "Check the internet connection!", Toast.LENGTH_SHORT).show()
                            }
                        })

                    }
                } else {
                    recyclerview_all_teams.visibility = View.GONE
                    emptyTxtView.visibility = View.VISIBLE
                    Toast.makeText(this@InboxActivity, response.message(), Toast.LENGTH_SHORT).show()
                }
            }

            override fun onFailure(call: Call<List<PrivateTeam>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(this@InboxActivity, "Check the internet connection!", Toast.LENGTH_SHORT).show()

            }
        })
    }

    private fun createJoinRequestRecyclerAdapter() {
        recyclerview_all_teams.visibility = View.VISIBLE
        emptyTxtView.visibility = View.GONE
        recyclerAdapter = RecyclerAdapterJoinRequests(baseContext, joinRequestsOfTeam, this@InboxActivity)
        recyclerAdapter.notifyDataSetChanged()
        recyclerview_all_teams.adapter = recyclerAdapter
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

    override fun onInboxClick(position: Int) {

    }
}