package nl.kaouch.jaouad.comakership.student.team

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.ImageView
import android.widget.TextView
import androidx.cardview.widget.CardView
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.floatingactionbutton.ExtendedFloatingActionButton
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterTeam
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.responses.SpecificTeam
import nl.kaouch.jaouad.comakership.models.responses.StudentUser
import nl.kaouch.jaouad.comakership.student.comakership.ComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class TeamDashboardActivity : AppCompatActivity(), RecyclerAdapterTeam.onTeamClickListener {

    private lateinit var recyclerview_joined_teams: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterTeam
    private lateinit var tokenManager: TokenManager
    private lateinit var responseBody: StudentUser
    private lateinit var cardlayout: CardView
    private lateinit var specificTeams: List<SpecificTeam?>
    private lateinit var joinedTeamIds: IntArray
    private lateinit var emptyTxtView: TextView
    private lateinit var team_name: TextView
    private lateinit var team_size_value: TextView
    private lateinit var constraint_layout: ConstraintLayout
    private lateinit var joinTeamBtn: ExtendedFloatingActionButton
    private lateinit var createTeamBtn: ExtendedFloatingActionButton
    private lateinit var privateTeamID: String
    private lateinit var logout_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_team_dashboard)

        tokenManager = TokenManager(applicationContext)
        createTeamBtn = findViewById(R.id.createTeamFab)
        joinTeamBtn = findViewById(R.id.joinTeamFab)
        cardlayout = findViewById(R.id.card_layout_private_team)
        emptyTxtView = findViewById(R.id.empty_txtview)
        team_name = findViewById(R.id.member_name)
        team_size_value = findViewById(R.id.team_description_value)
        constraint_layout = findViewById(R.id.constraint_layout)
        recyclerview_joined_teams = findViewById(R.id.recyclerview_student_all_teams)
        specificTeams = emptyList()
        joinedTeamIds = intArrayOf()
        privateTeamID = ""
        logout_button = findViewById(R.id.logout_button)

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@TeamDashboardActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent)
        }

        recyclerview_joined_teams.setHasFixedSize(true)
        recyclerview_joined_teams.layoutManager = LinearLayoutManager(this)

        getStudentUser(tokenManager.getToken(), tokenManager.getUserId())

        createTeamBtn.setOnClickListener(View.OnClickListener {
            val intent = Intent(this@TeamDashboardActivity, CreateTeamActivity::class.java)
            this.finish()
            startActivity(intent)
        })

        joinTeamBtn.setOnClickListener(View.OnClickListener {
            val intent = Intent(this@TeamDashboardActivity, AllTeamsActivity::class.java)
            intent.putExtra("joinedTeamIdList", joinedTeamIds)
            this.finish()
            startActivity(intent)
        })

        cardlayout.setOnClickListener{
            val intent = Intent(this@TeamDashboardActivity, SpecificTeamActivity::class.java)
            intent.putExtra("specificTeamID", privateTeamID.toInt())
            intent.putExtra("privateTeamID", privateTeamID.toInt())
            finish()
            startActivity(intent)
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_team
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@TeamDashboardActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@TeamDashboardActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@TeamDashboardActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@TeamDashboardActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@TeamDashboardActivity,
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

    private fun getStudentUser(token: String, id: Int) {
        val call = fetchApi().getStudentUser("Bearer " + token, id)
        call.enqueue(object : Callback<StudentUser> {

            override fun onResponse(
                call: Call<StudentUser>,
                response: Response<StudentUser>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@TeamDashboardActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBody = response.body()!!
                    team_name.text = responseBody.privateTeam.name
                    team_size_value.text = (responseBody.privateTeam.linkedStudents.size).toString()
                    joinedTeamIds += responseBody.privateTeamId
                    privateTeamID = responseBody.privateTeamId.toString()

                    if(response.body()!!.linkedTeams!!.isEmpty()) {
                        recyclerview_joined_teams.visibility = View.GONE
                        emptyTxtView.visibility = View.VISIBLE
                    } else {
                        responseBody.linkedTeams!!.forEach {
                            if(it.teamId != responseBody.privateTeamId) {
                                val call = fetchApi().getTeam("Bearer " + token, it.teamId)
                                call.enqueue(object : Callback<SpecificTeam> {

                                    override fun onResponse(
                                        call: Call<SpecificTeam>,
                                        response: Response<SpecificTeam>
                                    ) {
                                        if(response.code() == 401) {
                                            tokenManager.clearJwtToken()
                                            val intent = Intent(this@TeamDashboardActivity, LoginActivity::class.java)
                                            finish()
                                            startActivity(intent)
                                        }
                                        if (response.isSuccessful) {
                                            joinedTeamIds += response.body()!!.id
                                            specificTeams += response.body()
                                            createlinkedTeamRecyclerAdapter()
                                        }
                                    }

                                    override fun onFailure(call: Call<SpecificTeam>, t: Throwable) {
                                        Log.e("HTTP", "Could not fetch data", t)
                                    }
                                })
                            }
                        }
                    }
                }
            }
                override fun onFailure(call: Call<StudentUser>, t: Throwable) {
                    Log.e("HTTP", "Could not fetch data", t)
                }
            })
        }

    private fun createlinkedTeamRecyclerAdapter() {
        recyclerview_joined_teams.visibility = View.VISIBLE
        emptyTxtView.visibility = View.GONE
        recyclerAdapter = RecyclerAdapterTeam(baseContext, specificTeams, this@TeamDashboardActivity)
        recyclerAdapter.notifyDataSetChanged()
        recyclerview_joined_teams.adapter = recyclerAdapter
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
        val intent = Intent(this@TeamDashboardActivity, SpecificTeamActivity::class.java)
        intent.putExtra("specificTeamID", specificTeams[position]!!.id)
        this.finish()
        startActivity(intent)
    }
}