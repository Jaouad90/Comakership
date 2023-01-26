package nl.kaouch.jaouad.comakership.company.comakerships

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.*
import androidx.core.content.ContextCompat
import androidx.core.view.isVisible
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.chip.Chip
import com.google.android.material.chip.ChipGroup
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.models.Comakership
import nl.kaouch.jaouad.comakership.models.Deliverable
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.requests.PutComakership
import nl.kaouch.jaouad.comakership.models.responses.SpecificTeam
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class CompanyComakershipActivity : AppCompatActivity(), RecyclerAdapterTeamMembers.onTeamMemberClickListener, RecyclerAdapterCompanyDeliverableUpload.onDeliverableClickListener,
    AdapterView.OnItemSelectedListener {

    private lateinit var companyEditComakership: ImageView
    private lateinit var toolbarTitle: TextView
    private lateinit var tokenManager: TokenManager
    private lateinit var statusValue: TextView
    private lateinit var programValue: TextView
    private lateinit var titleValue: TextView
    private lateinit var descriptionValue: TextView
    private lateinit var skillChipGroup: ChipGroup
    private lateinit var deliverables: List<Deliverable>
    private lateinit var toolbar_back_button: ImageView
    private lateinit var recyclerview_deliverables: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterCompanyDeliverableUpload
    private lateinit var emptyTxtView1: TextView
    private lateinit var responseBodyDeliverables: List<Deliverable>
    private lateinit var responseBodyComakership: Comakership
    private lateinit var emptyTxtView: TextView
    private lateinit var recyclerview_teammembers: RecyclerView
    private lateinit var recyclerAdapterTeamMembers: RecyclerAdapterTeamMembers
    private lateinit var comakershipString: String
    private lateinit var dropdownStatusComakership: Spinner
    private var isFinished = 0
    private var listStatus = listOf<String>("Not started", "Started", "Finished")

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_company_comakership)

        val comakershipId = intent.getIntExtra("chosenComakershipId", 0)
        comakershipString = comakershipId.toString()
        tokenManager = TokenManager(applicationContext)

        deliverables = emptyList()
        statusValue = findViewById(R.id.comakership_status_valueview)
        programValue = findViewById(R.id.comakership_program_valueview)
        titleValue = findViewById(R.id.comakership_title_label)
        descriptionValue = findViewById(R.id.comakership_description_value)
        skillChipGroup = findViewById(R.id.comakership_chipgroup_skills)
        emptyTxtView1 = findViewById(R.id.empty_txtview1)
        recyclerview_deliverables = findViewById(R.id.recyclerview_student_comakership_deliverables)
        toolbarTitle = findViewById(R.id.toolbar_title)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_teammembers = findViewById(R.id.recyclerview_student_comakership_team)
        companyEditComakership = findViewById(R.id.edit_profile_company)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)
        dropdownStatusComakership = findViewById(R.id.comakership_status_dropdown)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@CompanyComakershipActivity, CompanyComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        recyclerview_teammembers.setHasFixedSize(true)
        recyclerview_teammembers.layoutManager = LinearLayoutManager(this)

        recyclerview_deliverables.setHasFixedSize(true)
        recyclerview_deliverables.layoutManager = LinearLayoutManager(this)

        companyEditComakership.setOnClickListener{
            dropdownStatusComakership.isVisible = dropdownStatusComakership.isVisible.equals(false)

            var adapter = ArrayAdapter(
                this,
                android.R.layout.simple_spinner_item,
                listStatus
            )
            adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
            dropdownStatusComakership.adapter = adapter
            dropdownStatusComakership.onItemSelectedListener =
                this
        }

        getComakershipDeliverables(tokenManager.getToken(), comakershipId)
        getComakerShip(tokenManager.getToken(), comakershipId)

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CompanyComakershipActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CompanyComakershipActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CompanyComakershipActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CompanyComakershipActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }

    private fun addChipToGroup(skill: String, chipGroup: ChipGroup) {
        if(chipGroup.checkedChipIds.size < 5) {
            val chip = Chip(this)
            chip.text = skill
            // necessary to get single selection working
            chip.isClickable = true
            chip.isCheckable = true
            chip.isChecked = true
            chip.isCheckedIconVisible = false
            chipGroup.addView(chip as View)
        }
    }

    private fun getComakershipDeliverables(token: String, comakershipId: Int) {
        val call = fetchApi().getComakershipDeliverables("Bearer "+token, comakershipId)
        call.enqueue(object : Callback<List<Deliverable>> {
            override fun onResponse(
                call: Call<List<Deliverable>>,
                response: Response<List<Deliverable>>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyComakershipActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBodyDeliverables = response.body()!!
                    if(responseBodyDeliverables!!.isEmpty()) {
                        recyclerview_deliverables.visibility = View.GONE
                        emptyTxtView1.visibility = View.VISIBLE
                    } else {
                        recyclerview_deliverables.visibility = View.VISIBLE
                        emptyTxtView1.visibility = View.GONE
                        recyclerAdapter = RecyclerAdapterCompanyDeliverableUpload(baseContext, responseBodyDeliverables, this@CompanyComakershipActivity)
                        recyclerAdapter.notifyDataSetChanged()
                        recyclerview_deliverables.adapter = recyclerAdapter
                    }
                }
            }
            override fun onFailure(call: Call<List<Deliverable>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
    }

    private fun getComakerShip(token: String, id: Int) {
        var programs = ""
        val call = fetchApi().getComakership("Bearer "+token, id)
        call.enqueue(object : Callback<Comakership> {

            override fun onResponse(
                call: Call<Comakership>,
                response: Response<Comakership>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyComakershipActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBodyComakership = response.body()!!
                    isFinished = responseBodyComakership.status!!.id
                    getComakershipDeliverables(token, id)
                    toolbarTitle.text = responseBodyComakership!!.name
                    statusValue.text = responseBodyComakership.status!!.name
                    when (responseBodyComakership.status!!.id) {
                        1 -> {
                            statusValue.setTextColor(ContextCompat.getColor(this@CompanyComakershipActivity, R.color.status_red))
                        }
                        2 -> {
                            statusValue.setTextColor(ContextCompat.getColor(this@CompanyComakershipActivity, R.color.status_orange))
                        }
                        3 -> {
                            statusValue.setTextColor(ContextCompat.getColor(this@CompanyComakershipActivity, R.color.primary_green))
                        }
                    }
                    statusValue.text = responseBodyComakership!!.status!!.name
                    responseBodyComakership.programs.forEach{
                        programs += " "+it.name.toString()+" | "
                    }
                    programValue.text = programs
                    titleValue.text = responseBodyComakership.name
                    descriptionValue.text = responseBodyComakership.description
                    responseBodyComakership.skills.forEach {
                        addChipToGroup(it.name, skillChipGroup)
                    }
                    val call = fetchApi().getTeam("Bearer "+token, id)
                    call.enqueue(object : Callback<SpecificTeam> {

                        override fun onResponse(
                            call: Call<SpecificTeam>,
                            response: Response<SpecificTeam>
                        ) {
                            if(response.code() == 401) {
                                tokenManager.clearJwtToken()
                                val intent = Intent(this@CompanyComakershipActivity, LoginActivity::class.java)
                                finish()
                                startActivity(intent)
                            }
                            if (response.isSuccessful) {
                                val responseBody = response.body()
                                if(responseBody!!.members.isEmpty()) {
                                    recyclerview_teammembers.visibility = View.GONE
                                    emptyTxtView.visibility = View.VISIBLE
                                } else {
                                    recyclerview_teammembers.visibility = View.VISIBLE
                                    emptyTxtView.visibility = View.GONE
                                    recyclerAdapterTeamMembers = RecyclerAdapterTeamMembers(baseContext, responseBody.members, this@CompanyComakershipActivity)
                                    recyclerAdapterTeamMembers.notifyDataSetChanged()
                                    recyclerview_teammembers.adapter = recyclerAdapterTeamMembers
                                }
                            }
                        }
                        override fun onFailure(call: Call<SpecificTeam>, t: Throwable) {
                            Log.e("HTTP", "Could not fetch data", t);
                        }
                    })
                }
            }
            override fun onFailure(call: Call<Comakership>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
    }

    private fun fetchStatus() {
        val call = fetchApi().updateComakershipStatus(
            "Bearer " + tokenManager.getToken(),
            comakershipString.toInt(),
            PutComakership(responseBodyComakership.id!!,responseBodyComakership.name,responseBodyComakership.description,responseBodyComakership.credits,responseBodyComakership.bonus,isFinished)
        )
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyComakershipActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    dropdownStatusComakership.isVisible = false
                    Toast.makeText(this@CompanyComakershipActivity, "Comakership status updated successfully!", Toast.LENGTH_SHORT)
                        .show()
                    val intent = Intent(this@CompanyComakershipActivity, CompanyComakershipDashboardActivity::class.java)
                    finish()
                    startActivity(intent)
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(this@CompanyComakershipActivity, "Check the internet connection!", Toast.LENGTH_SHORT)
                    .show()
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

    override fun onDeliverableClick(position: Int) {
        if(responseBodyDeliverables[position].finished == false) {
            val intent = Intent(
                this,
                CompanyComakershipDeliverableAlertDialogActivity::class.java
            )

            intent.putExtra("chosenDeliverableId", responseBodyDeliverables[position].id)
            this.finish()
            this.startActivity(intent)
        }
    }

    override fun onTeamMemberClick(position: Int) {
    }

    override fun onItemSelected(p0: AdapterView<*>?, p1: View?, p2: Int, p3: Long) {
        when {
            listStatus[p2]=="Not Started" -> {
                isFinished = 1
                fetchStatus()
            }
            listStatus[p2]=="Started" -> {
                isFinished = 2
                fetchStatus()
            }
            listStatus[p2]=="Finished" -> {
                isFinished = 3
                fetchStatus()
            }
        }
    }

    override fun onNothingSelected(p0: AdapterView<*>?) {
    }
}