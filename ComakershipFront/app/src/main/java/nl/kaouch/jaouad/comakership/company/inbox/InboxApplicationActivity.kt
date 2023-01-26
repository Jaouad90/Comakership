package nl.kaouch.jaouad.comakership.company.inbox

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterTeamApplications
import nl.kaouch.jaouad.comakership.company.comakerships.CompanyComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Comakership
import nl.kaouch.jaouad.comakership.models.requests.PutComakership
import nl.kaouch.jaouad.comakership.models.responses.ComakershipApplications
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class InboxApplicationActivity : AppCompatActivity(), RecyclerAdapterTeamApplications.onApplicationClickListener  {

    private lateinit var recyclerview_applications: RecyclerView
    private lateinit var recyclerAdapterApplication: RecyclerAdapterTeamApplications
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var toolbar_back_button: ImageView


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_inbox_application)

        tokenManager = TokenManager(applicationContext)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_applications = findViewById(R.id.recyclerview_company_applications)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@InboxApplicationActivity, CompanyInboxDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        recyclerview_applications.setHasFixedSize(true)
        recyclerview_applications.layoutManager = LinearLayoutManager(this)

        getComakership(tokenManager.getToken())

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_inbox
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@InboxApplicationActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@InboxApplicationActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@InboxApplicationActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@InboxApplicationActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }
    private fun getComakership(token: String) {
        val comakershipId = intent.getIntExtra("chosenComakershipId", 0)

        val call = fetchApi().getComakership("Bearer "+ token, comakershipId)
        call.enqueue(object : Callback<Comakership> {

            override fun onResponse(
                call: Call<Comakership>,
                response: Response<Comakership>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@InboxApplicationActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    var x = response.body()
                    var putComakership = PutComakership (
                    x!!.id!!,
                    x.name,
                    x.description,
                    x.bonus,
                    x.credits,
                    x.status!!.id)

                    getApplications(token, comakershipId, putComakership)
                }
            }
            override fun onFailure(call: Call<Comakership>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
    }

    private fun getApplications(token: String, comakershipId: Int, putComakership: PutComakership) {
        val call1 = fetchApi().getApplicationsOfComakership("Bearer "+ token, comakershipId
        )
        call1.enqueue(object : Callback<List<ComakershipApplications>> {

            override fun onResponse(
                call: Call<List<ComakershipApplications>>,
                response: Response<List<ComakershipApplications>>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@InboxApplicationActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    var applications = response.body()
                    if(applications!!.isEmpty()) {
                        recyclerview_applications.visibility = View.GONE
                        emptyTxtView.visibility = View.VISIBLE
                    } else {
                        recyclerview_applications.visibility = View.VISIBLE
                        emptyTxtView.visibility = View.GONE
                        recyclerAdapterApplication = RecyclerAdapterTeamApplications(putComakership, baseContext, applications, this@InboxApplicationActivity)
                        recyclerAdapterApplication.notifyDataSetChanged()
                        recyclerview_applications.adapter = recyclerAdapterApplication
                    }
                }
            }
            override fun onFailure(call: Call<List<ComakershipApplications>>, t: Throwable) {
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

    override fun onApplicationClick(position: Int) {
    }
}