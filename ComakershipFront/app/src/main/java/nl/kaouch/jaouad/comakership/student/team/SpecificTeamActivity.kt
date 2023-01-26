package nl.kaouch.jaouad.comakership.student.team

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.view.View.GONE
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.core.view.isVisible
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.responses.SpecificTeam
import nl.kaouch.jaouad.comakership.student.comakership.ComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class SpecificTeamActivity : AppCompatActivity(), RecyclerAdapterTeamMembers.onTeamMemberClickListener {

    private lateinit var recyclerview_team_members: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterTeamMembers
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var toolBarTitle: TextView
    private lateinit var teamDescription: TextView
    private lateinit var toolbar_back_button: ImageView
    private lateinit var leaveBtn: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_specific_team)

        tokenManager = TokenManager(applicationContext)
        emptyTxtView = findViewById(R.id.empty_txtview)
        toolBarTitle = findViewById(R.id.toolbar_title)
        teamDescription = findViewById(R.id.specificteam_description_txtview)
        leaveBtn = findViewById(R.id.leave_team_btn)
        recyclerview_team_members = findViewById(R.id.team_members_recyclerview)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        recyclerview_team_members.setHasFixedSize(true)
        recyclerview_team_members.layoutManager = LinearLayoutManager(this)

        var teamID = intent.getIntExtra ("specificTeamID", -1)
        var privateteamID = intent.getIntExtra ("privateTeamID", -1)
        if(teamID == privateteamID) {
            leaveBtn.isVisible = false
        }
        getTeam(tokenManager.getToken(), teamID)

        leaveBtn.setOnClickListener{
            leaveTeam(tokenManager.getToken(), teamID)
        }

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@SpecificTeamActivity, TeamDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_team
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@SpecificTeamActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@SpecificTeamActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@SpecificTeamActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@SpecificTeamActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@SpecificTeamActivity,
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

    private fun leaveTeam(token: String, id: Int) {
        val call = fetchApi().leaveTeam("Bearer " + token, id)
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@SpecificTeamActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    Toast.makeText(
                            this@SpecificTeamActivity,
                            "Left team succesfully!!",
                            Toast.LENGTH_SHORT
                    ).show()
                    val intent = Intent(this@SpecificTeamActivity, TeamDashboardActivity::class.java)
                    finish()
                    startActivity(intent)
                } else {
                    Toast.makeText(
                            this@SpecificTeamActivity,
                            "You can't leave your private team!!",
                            Toast.LENGTH_SHORT
                    ).show()
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
            }
        })
    }

    private fun getTeam(token: String, id: Int) {
        val call = fetchApi().getTeam("Bearer " + token, id)
        call.enqueue(object : Callback<SpecificTeam> {

            override fun onResponse(
                call: Call<SpecificTeam>,
                response: Response<SpecificTeam>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@SpecificTeamActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    var responseBody = response.body()
                    toolBarTitle.text = responseBody!!.name
                    teamDescription.text = responseBody.description
                    if(response.body()!!.members.isEmpty()) {
                        recyclerview_team_members.visibility = GONE
                        emptyTxtView.visibility = View.VISIBLE
                    } else {
                        recyclerview_team_members.visibility = View.VISIBLE
                        emptyTxtView.visibility = GONE
                        recyclerAdapter = RecyclerAdapterTeamMembers(baseContext, responseBody.members, this@SpecificTeamActivity)
                        recyclerAdapter.notifyDataSetChanged()
                        recyclerview_team_members.adapter = recyclerAdapter
                    }
                }
            }

            override fun onFailure(call: Call<SpecificTeam>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
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

    override fun onTeamMemberClick(position: Int) {
    }
}