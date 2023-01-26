package nl.kaouch.jaouad.comakership.company.profile

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.isVisible
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.models.Company
import nl.kaouch.jaouad.comakership.models.responses.CompanyUser
import nl.kaouch.jaouad.comakership.models.Review
import nl.kaouch.jaouad.comakership.company.comakerships.CompanyComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import kotlin.properties.Delegates


class CompanyProfileDashboardActivity : AppCompatActivity() {

    private lateinit var companyUserProfile: ImageView
    private lateinit var companyEditProfile: ImageView
    private lateinit var company_id: TextView
    private lateinit var company_name: TextView
    private lateinit var company_description: TextView
    private lateinit var company_registrationDate: TextView
    private lateinit var company_street: TextView
    private lateinit var company_city: TextView
    private lateinit var company_zipcode: TextView
    private lateinit var company_id_edit: EditText
    private lateinit var company_name_edit: EditText
    private lateinit var company_description_edit: EditText
    private lateinit var company_registrationDate_edit: EditText
    private lateinit var company_street_edit: EditText
    private lateinit var company_city_edit: EditText
    private lateinit var company_zipcode_edit: EditText
    private lateinit var company_profile_save_btn: Button
    private lateinit var reviews: List<Review>
    private lateinit var recyclerview_reviews: RecyclerView
    private var companyId by Delegates.notNull<Int>()
    private lateinit var company: Company
    private lateinit var companyUser: CompanyUser
    private lateinit var recyclerAdapter: RecyclerAdapterReview
    private lateinit var tokenManager: TokenManager
    private lateinit var logout_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_company_profile)

        recyclerview_reviews = findViewById(R.id.recyclerview_reviews)
        recyclerview_reviews.setHasFixedSize(true)
        recyclerview_reviews.layoutManager = LinearLayoutManager(this)

        tokenManager = TokenManager(applicationContext)
        reviews = emptyList()
        companyUserProfile = findViewById(R.id.user_profile_company)
        company_id = findViewById(R.id.company_id_txtview_value)
        company_name = findViewById(R.id.company_name_txtview_value)
        company_description = findViewById(R.id.company_description_txtview_value)
        company_registrationDate = findViewById(R.id.company_registrationDate_txtview_value)
        company_street = findViewById(R.id.company_street_txtview_value)
        company_city = findViewById(R.id.company_city_txtview_value)
        company_zipcode = findViewById(R.id.company_zipcode_txtview_value)
        companyEditProfile = findViewById(R.id.edit_profile_company)
        company_profile_save_btn = findViewById(R.id.company_profile_save_btn)

        company_id_edit = findViewById(R.id.company_id_edittext)
        company_name_edit = findViewById(R.id.company_name_edittext)
        company_description_edit = findViewById(R.id.company_description_edittext)
        company_registrationDate_edit = findViewById(R.id.company_registrationDate_edittext)
        company_street_edit = findViewById(R.id.company_street_edittext)
        company_city_edit = findViewById(R.id.company_city_edittext)
        company_zipcode_edit = findViewById(R.id.company_zipcode_edittext)
        logout_button = findViewById(R.id.logout_button)

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@CompanyProfileDashboardActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent) }

        companyUserProfile.setOnClickListener {
            val intent = Intent(
                this@CompanyProfileDashboardActivity,
                CompanyUserProfileActivity::class.java
            )
            intent.putExtra("companyUser", companyUser)
            finish()
            startActivity(intent)
        }

        companyEditProfile.setOnClickListener {
            if(!company_name_edit.isEnabled) {
                company_id_edit.isEnabled = false
                company_id_edit.isVisible = false
                company_name_edit.isEnabled = true
                company_name_edit.isVisible = true
                company_description_edit.isEnabled = true
                company_description_edit.isVisible = true
                company_registrationDate_edit.isEnabled = false
                company_registrationDate_edit.isVisible = false
                company_street_edit.isEnabled = true
                company_street_edit.isVisible = true
                company_city_edit.isEnabled = true
                company_city_edit.isVisible = true
                company_zipcode_edit.isEnabled = true
                company_zipcode_edit.isVisible = true
                company_profile_save_btn.isEnabled = true
                company_profile_save_btn.isVisible = true
            } else {
                company_name_edit.isEnabled = false
                company_name_edit.isVisible = false
                company_description_edit.isEnabled = false
                company_description_edit.isVisible = false
                company_street_edit.isEnabled = false
                company_street_edit.isVisible = false
                company_city_edit.isEnabled = false
                company_city_edit.isVisible = false
                company_zipcode_edit.isEnabled = false
                company_zipcode_edit.isVisible = false
                company_profile_save_btn.isEnabled = false
                company_profile_save_btn.isVisible = false
            }
        }

        company_profile_save_btn.setOnClickListener {
            var company = Company(
                company.id,
                if (company_name_edit.text.toString()
                        .isNotEmpty()
                ) company_name_edit.text.toString() else company.name,
                if (company_description_edit.text.toString()
                        .isNotEmpty()
                ) company_description_edit.text.toString() else company.description,
                if (company_registrationDate_edit.text.toString()
                        .isNotEmpty()
                ) company_registrationDate_edit.text.toString() else company.registrationDate,
                company.reviews,
                company.comakerships,
                if (company_street_edit.text.toString()
                        .isNotEmpty()
                ) company_street_edit.text.toString() else company.street,
                if (company_city_edit.text.toString()
                        .isNotEmpty()
                ) company_city_edit.text.toString() else company.city,
                if (company_zipcode_edit.text.toString()
                        .isNotEmpty()
                ) company_zipcode_edit.text.toString() else company.zipcode,
                company.CompanyUser,
            )

            val call = fetchApi().setCompanyInfo(
                "Bearer " + tokenManager.getToken(),
                companyId,
                company
            )
            call.enqueue(object : Callback<Void> {

                override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
                ) {
                    if(response.code() == 401) {
                        tokenManager.clearJwtToken()
                        val intent = Intent(this@CompanyProfileDashboardActivity, LoginActivity::class.java)
                        finish()
                        startActivity(intent)
                    }
                    if (response.isSuccessful) {
                        val responseBody = response.body()
                        println(responseBody)
                        Toast.makeText(
                            this@CompanyProfileDashboardActivity,
                            "Company profile updated succesfully!!",
                            Toast.LENGTH_SHORT
                        ).show()
                        finish()
                        startActivity(getIntent())
                    } else if (response.code() == 409) {

                        Toast.makeText(
                            this@CompanyProfileDashboardActivity,
                            response.errorBody()!!.string(),
                            Toast.LENGTH_LONG
                        ).show()
                        val intent = Intent(
                            this@CompanyProfileDashboardActivity,
                            CompanyProfileDashboardActivity::class.java
                        )
                        finish()
                        startActivity(intent)
                    }
                }

                override fun onFailure(call: Call<Void>, t: Throwable) {
                    Log.e("HTTP", "Could not fetch data", t);
                }
            })
        }

        getCompanyUser(tokenManager.getToken(), tokenManager.getUserId())

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_profile
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CompanyProfileDashboardActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CompanyProfileDashboardActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CompanyProfileDashboardActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CompanyProfileDashboardActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }

    private fun getCompanyUser(token: String, id: Int) {
        val call = fetchApi().getCompanyUser("Bearer " + token, id)
        call.enqueue(object : Callback<CompanyUser> {

            override fun onResponse(
                call: Call<CompanyUser>,
                response: Response<CompanyUser>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyProfileDashboardActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    val responseBody = response.body()
                    company = responseBody!!.company!!
                    companyUser = responseBody
                    companyId = responseBody!!.company!!.id!!
                    getCompany(token, companyId)
                }
            }

            override fun onFailure(call: Call<CompanyUser>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
    }

    private fun getCompany(token: String, id: Int) {
        val call = fetchApi().getCompany("Bearer " + token, id)
        call.enqueue(object : Callback<Company> {

            override fun onResponse(
                call: Call<Company>,
                response: Response<Company>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyProfileDashboardActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    val responseBody = response.body()
                    company_id.text = responseBody!!.id.toString()
                    company_name.text = responseBody.name
                    company_description.text = responseBody.description
                    company_registrationDate.text = responseBody.registrationDate!!.substring(
                        0, responseBody.registrationDate.indexOf(
                            "T"
                        )
                    )
                    company_street.text = responseBody.street
                    company_city.text = responseBody.city
                    company_zipcode.text = responseBody.zipcode
                    recyclerAdapter = RecyclerAdapterReview(baseContext, responseBody.reviews)
                    recyclerAdapter.notifyDataSetChanged()
                    recyclerview_reviews.adapter = recyclerAdapter
                }
            }

            override fun onFailure(call: Call<Company>, t: Throwable) {
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
}