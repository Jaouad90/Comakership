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
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.company.comakerships.CompanyComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Comakership
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class CompanyInboxDashboardActivity : AppCompatActivity(), RecyclerAdapterInboxComakerships.onComakershipClickListener{

    private lateinit var recyclerview_comakerships: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterInboxComakerships
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var responseBody: List<Comakership>
    private lateinit var logout_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_company_inbox)

        tokenManager = TokenManager(applicationContext)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_comakerships = findViewById(R.id.recyclerview_company_comakerships)
        logout_button = findViewById(R.id.logout_button)

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@CompanyInboxDashboardActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent) }

        recyclerview_comakerships.setHasFixedSize(true)
        recyclerview_comakerships.layoutManager = LinearLayoutManager(this)

        getComakerShips(tokenManager.getToken())

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_inbox
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CompanyInboxDashboardActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CompanyInboxDashboardActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CompanyInboxDashboardActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CompanyInboxDashboardActivity, CompanyComakershipDashboardActivity::class.java)
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
                    val intent = Intent(this@CompanyInboxDashboardActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBody = response.body()!!
                    if(responseBody!!.isEmpty()) {
                        recyclerview_comakerships.visibility = View.GONE
                        emptyTxtView.visibility = View.VISIBLE
                    } else {
                        recyclerview_comakerships.visibility = View.VISIBLE
                        emptyTxtView.visibility = View.GONE
                        recyclerAdapter = RecyclerAdapterInboxComakerships(baseContext, responseBody, this@CompanyInboxDashboardActivity)
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
        val intent = Intent(this@CompanyInboxDashboardActivity, InboxApplicationActivity::class.java)
        intent.removeExtra("chosenComakershipId")
        intent.putExtra("chosenComakershipId", responseBody[position].id)
        finish()
        startActivity(intent)
    }
}