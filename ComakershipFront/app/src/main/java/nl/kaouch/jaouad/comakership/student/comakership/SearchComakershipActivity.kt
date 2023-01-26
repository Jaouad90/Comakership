package nl.kaouch.jaouad.comakership.student.comakership

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
import com.google.android.material.textfield.TextInputEditText
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterSearchComakership
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

class SearchComakershipActivity : AppCompatActivity(), RecyclerAdapterSearchComakership.onComakershipClickListener {

    private lateinit var recyclerview_comakerships: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterSearchComakership
    private lateinit var tokenManager: TokenManager
    private lateinit var searchComakershipBtn: ImageView
    private lateinit var emptyTxtView: TextView
    private lateinit var skillEditText: TextInputEditText
    private lateinit var responseBody: List<Comakership>
    private lateinit var toolbarBackButton: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_search_comakership)

        recyclerview_comakerships = findViewById(R.id.recyclerview_comakerships_search)
        emptyTxtView = findViewById(R.id.empty_txtview)
        skillEditText = findViewById(R.id.skill_et)
        searchComakershipBtn = findViewById(R.id.add_comakership_btn)
        responseBody = emptyList()
        toolbarBackButton = findViewById(R.id.toolbar_back_button)

        toolbarBackButton.setOnClickListener {
            val intent = Intent(this@SearchComakershipActivity, ComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        recyclerview_comakerships.setHasFixedSize(true)
        recyclerview_comakerships.layoutManager = LinearLayoutManager(this)

        tokenManager = TokenManager(applicationContext)

        searchComakershipBtn.setOnClickListener {
            searchComakerships()
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@SearchComakershipActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@SearchComakershipActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@SearchComakershipActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@SearchComakershipActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@SearchComakershipActivity,
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

    private fun searchComakerships() {
        var skill = skillEditText.text.toString()
        val call = fetchApi().searchComakerships(skill)
        call.enqueue(object : Callback<List<Comakership>> {

            override fun onResponse(
                call: Call<List<Comakership>>,
                response: Response<List<Comakership>>
            ) {
                if (response.isSuccessful) {
                    responseBody = response.body()!!
                    if(responseBody!!.isEmpty()) {
                        recyclerview_comakerships.visibility = View.GONE
                        emptyTxtView.visibility = View.VISIBLE
                    } else {
                        recyclerview_comakerships.visibility = View.VISIBLE
                        emptyTxtView.visibility = View.GONE
                        recyclerAdapter = RecyclerAdapterSearchComakership(baseContext, responseBody, this@SearchComakershipActivity)
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
    }
}