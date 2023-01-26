package nl.kaouch.jaouad.comakership.student.team

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterAllTeams
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.PrivateTeam
import nl.kaouch.jaouad.comakership.student.comakership.ComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class AllTeamsActivity : AppCompatActivity(), RecyclerAdapterAllTeams.onTeamClickListener {

    private lateinit var recyclerview_all_teams: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterAllTeams
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var joinableTeams: List<PrivateTeam?>
    private lateinit var toolbar_back_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_all_teams)

        tokenManager = TokenManager(applicationContext)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_all_teams = findViewById(R.id.recyclerview_student_all_teams)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)
        joinableTeams = emptyList()

        recyclerview_all_teams.setHasFixedSize(true)
        recyclerview_all_teams.layoutManager = LinearLayoutManager(this)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@AllTeamsActivity, TeamDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        getTeams(tokenManager.getToken())

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_team
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@AllTeamsActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@AllTeamsActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@AllTeamsActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@AllTeamsActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@AllTeamsActivity,
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

    private fun createAllTeamRecyclerAdapter() {
        recyclerview_all_teams.visibility = View.VISIBLE
        emptyTxtView.visibility = View.GONE
        recyclerAdapter = RecyclerAdapterAllTeams(baseContext, joinableTeams, this@AllTeamsActivity)
        recyclerAdapter.notifyDataSetChanged()
        recyclerview_all_teams.adapter = recyclerAdapter
    }

    private fun getTeams(token: String) {
        val call = fetchApi().getTeams("Bearer " + token)
        call.enqueue(object : Callback<List<PrivateTeam>> {

            override fun onResponse(
                call: Call<List<PrivateTeam>>,
                response: Response<List<PrivateTeam>>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@AllTeamsActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    if (response.body()!!.isEmpty()) {
                        recyclerview_all_teams.visibility = View.GONE
                        emptyTxtView.visibility = View.VISIBLE
                    } else {

                        var teams = response.body()
                        var joinedTeamIds = intent.getIntArrayExtra("joinedTeamIdList")
                        teams = teams!!.toMutableList()
                        joinedTeamIds!!.forEach {
                            teams.removeAll { item ->
                                item.id == it
                            }
                        }
                        joinableTeams = teams
                        createAllTeamRecyclerAdapter()
                    }
                }
            }

            override fun onFailure(call: Call<List<PrivateTeam>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(
                    this@AllTeamsActivity,
                    "Check the internet connection!",
                    Toast.LENGTH_SHORT
                ).show()

            }
        })
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

    override fun onTeamClick(position: Int) {

    }
}