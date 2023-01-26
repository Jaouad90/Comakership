package nl.kaouch.jaouad.comakership.company.comakerships

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
import com.google.android.material.floatingactionbutton.ExtendedFloatingActionButton
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterComakership
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Comakership
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class CompanyComakershipDashboardActivity : AppCompatActivity(), RecyclerAdapterComakership.onComakershipClickListener {

    private lateinit var recyclerview_comakerships: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterComakership
    private lateinit var tokenManager: TokenManager
    private lateinit var emptyTxtView: TextView
    private lateinit var floatBtn: ExtendedFloatingActionButton
    private lateinit var responseBody: List<Comakership>
    private lateinit var logout_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_company_comakership_dashboard)

        responseBody = emptyList()
        recyclerview_comakerships = findViewById(R.id.recyclerview_comakerships_dashboard)
        emptyTxtView = findViewById(R.id.empty_txtview)
        floatBtn = findViewById(R.id.createComakershipFab)
        logout_button = findViewById(R.id.logout_button)

        recyclerview_comakerships.setHasFixedSize(true)
        recyclerview_comakerships.layoutManager = LinearLayoutManager(this)

        tokenManager = TokenManager(applicationContext)
        getComakerShips(tokenManager.getToken())

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CompanyComakershipDashboardActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CompanyComakershipDashboardActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CompanyComakershipDashboardActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CompanyComakershipDashboardActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }

        floatBtn.setOnClickListener(View.OnClickListener {
            val intent = Intent(this@CompanyComakershipDashboardActivity, CreateComakershipActivity::class.java)
            this.finish()
            startActivity(intent)
        })

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@CompanyComakershipDashboardActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent) }
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
                    val intent = Intent(this@CompanyComakershipDashboardActivity, LoginActivity::class.java)
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
                        recyclerAdapter = RecyclerAdapterComakership(baseContext, responseBody, this@CompanyComakershipDashboardActivity)
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
        val intent = Intent(this@CompanyComakershipDashboardActivity, CompanyComakershipActivity::class.java)
        intent.putExtra("chosenComakershipId", responseBody[position].id)
        this.finish()
        startActivity(intent)
    }
}