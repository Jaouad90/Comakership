package nl.kaouch.jaouad.comakership.student.comakership

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.ImageView
import android.widget.TextView
import androidx.core.view.isVisible
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.floatingactionbutton.ExtendedFloatingActionButton
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterComakership
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Comakership
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import nl.kaouch.jaouad.comakership.student.team.TeamDashboardActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class ComakershipDashboardActivity : AppCompatActivity(), RecyclerAdapterComakership.onComakershipClickListener {

    private lateinit var recyclerview_comakerships: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterComakership
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var logout_button: ImageView
    private lateinit var searchProjectBtn: ExtendedFloatingActionButton
    private lateinit var responseBody: List<Comakership>


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_comakership_dashboard)

        tokenManager = TokenManager(applicationContext)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_comakerships = findViewById(R.id.recyclerview_dash_student_comakerships)
        logout_button = findViewById(R.id.logout_button)
        searchProjectBtn = findViewById(R.id.searchComakershipsFab)

        recyclerview_comakerships.setHasFixedSize(true)
        recyclerview_comakerships.layoutManager = LinearLayoutManager(this)

        getComakerShips(tokenManager.getToken())

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@ComakershipDashboardActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent)
        }

        searchProjectBtn.setOnClickListener(View.OnClickListener {
            val intent = Intent(this@ComakershipDashboardActivity, SearchComakershipActivity::class.java)
            this.finish()
            startActivity(intent)
        })

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@ComakershipDashboardActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@ComakershipDashboardActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@ComakershipDashboardActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@ComakershipDashboardActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@ComakershipDashboardActivity,
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

    private fun getComakerShips(token: String) {
        val call = fetchApi().getComakerShipsOfUser("Bearer "+token)
        call.enqueue(object : Callback<List<Comakership>> {

            override fun onResponse(
                call: Call<List<Comakership>>,
                response: Response<List<Comakership>>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@ComakershipDashboardActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBody = response.body()!!
                    if(responseBody!!.isEmpty()) {
                        recyclerview_comakerships.isVisible = false
                        emptyTxtView.isVisible = true
                    } else {
                        recyclerview_comakerships.isVisible = true
                        emptyTxtView.isVisible = false
                        recyclerAdapter = RecyclerAdapterComakership(baseContext, responseBody, this@ComakershipDashboardActivity)
                        recyclerAdapter.notifyDataSetChanged()
                        recyclerview_comakerships.adapter = recyclerAdapter
                    }
                }
            }
            override fun onFailure(call: Call<List<Comakership>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
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

    override fun onComakershipClick(position: Int) {
        val intent = Intent(this@ComakershipDashboardActivity, StudentComakershipActivity::class.java)
        intent.removeExtra("chosenComakershipId")
        intent.putExtra("chosenComakershipId", responseBody[position].id)
        finish()
        startActivity(intent)
    }
}