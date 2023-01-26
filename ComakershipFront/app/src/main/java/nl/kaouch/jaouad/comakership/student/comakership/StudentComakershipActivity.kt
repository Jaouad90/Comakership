package nl.kaouch.jaouad.comakership.student.comakership

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.*
import androidx.core.content.ContextCompat
import androidx.core.view.isVisible
import androidx.core.widget.NestedScrollView
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.chip.Chip
import com.google.android.material.chip.ChipGroup
import com.google.android.material.textfield.TextInputEditText
import com.google.android.material.textfield.TextInputLayout
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Comakership
import nl.kaouch.jaouad.comakership.models.Deliverable
import nl.kaouch.jaouad.comakership.models.requests.PostReview
import nl.kaouch.jaouad.comakership.models.responses.SpecificTeam
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import nl.kaouch.jaouad.comakership.student.team.TeamDashboardActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class StudentComakershipActivity : AppCompatActivity(), RecyclerAdapterTeamMembers.onTeamMemberClickListener, RecyclerAdapterDeliverableUpload.onDeliverableClickListener {

    private lateinit var tokenManager: TokenManager
    private lateinit var toolbar_back_button: ImageView
    private lateinit var statusValue: TextView
    private lateinit var programValue: TextView
    private lateinit var titleValue: TextView
    private lateinit var descriptionValue: TextView
    private lateinit var skillChipGroup: ChipGroup
    private lateinit var recyclerview_deliverables: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterDeliverableUpload
    private lateinit var emptyTxtView1: TextView
    private lateinit var toolbarTitle: TextView
    private lateinit var emptyTxtView: TextView
    private lateinit var recyclerview_teammembers: RecyclerView
    private lateinit var recyclerAdapterTeamMembers: RecyclerAdapterTeamMembers
    private lateinit var comakershipString: String
    private lateinit var responseBody: List<Deliverable>

    private lateinit var rewardValue: TextView
    private lateinit var rewardText: TextView
    private lateinit var startNScrollV: NestedScrollView
    private lateinit var endNScrollV: NestedScrollView

    private lateinit var reviewTitle: TextView
    private lateinit var reviewCommentTitle: TextView
    private lateinit var reviewComment: TextInputEditText
    private lateinit var reviewTxtinputlayout: TextInputLayout
    private lateinit var ratingbar: RatingBar
    private lateinit var submitBtn: Button

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

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_student_comakership)


        val comakershipId: Int? = intent.getIntExtra("chosenComakershipId", 0)
        comakershipString = comakershipId.toString()
        tokenManager = TokenManager(applicationContext)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)
        statusValue = findViewById(R.id.comakership_status_valueview)
        programValue = findViewById(R.id.comakership_program_valueview)
        titleValue = findViewById(R.id.comakership_title_label)
        descriptionValue = findViewById(R.id.comakership_description_value)
        skillChipGroup = findViewById(R.id.comakership_chipgroup_skills)
        toolbarTitle = findViewById(R.id.toolbar_title)
        emptyTxtView = findViewById(R.id.empty_txtview)
        recyclerview_teammembers = findViewById(R.id.recyclerview_student_comakership_team)
        emptyTxtView1 = findViewById(R.id.empty_txtview1)
        recyclerview_deliverables = findViewById(R.id.recyclerview_student_comakership_deliverables)
        responseBody = emptyList()

        rewardValue = findViewById(R.id.reward_label)
        rewardText = findViewById(R.id.reward_text)
        startNScrollV = findViewById(R.id.student_comakership_nestedscrollview_active)
        endNScrollV = findViewById(R.id.student_comakership_nestedscrollview_inactive)

        reviewTitle = findViewById(R.id.comakership_review_title)
        reviewCommentTitle = findViewById(R.id.review_comment_title)
        reviewComment = findViewById(R.id.student_review_edittext)
        reviewTxtinputlayout = findViewById(R.id.student_review_txtinputlayout)
        ratingbar = findViewById(R.id.ratingBar1)
        submitBtn = findViewById(R.id.SubmitReview)

        recyclerview_teammembers.setHasFixedSize(true)
        recyclerview_teammembers.layoutManager = LinearLayoutManager(this)

        recyclerview_deliverables.setHasFixedSize(true)
        recyclerview_deliverables.layoutManager = LinearLayoutManager(this)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@StudentComakershipActivity, ComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        getComakerShip(tokenManager.getToken(), comakershipId!!)

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@StudentComakershipActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@StudentComakershipActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@StudentComakershipActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@StudentComakershipActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@StudentComakershipActivity,
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

    private fun getComakershipDeliverables(token: String, comakershipId: Int) {
        val call = fetchApi().getComakershipDeliverables("Bearer "+token, comakershipId)
        call.enqueue(object : Callback<List<Deliverable>> {
            override fun onResponse(
                call: Call<List<Deliverable>>,
                response: Response<List<Deliverable>>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@StudentComakershipActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBody = response.body()!!
                    if(responseBody!!.isEmpty()) {
                        recyclerview_deliverables.visibility = View.GONE
                        emptyTxtView1.visibility = View.VISIBLE
                    } else {
                        recyclerview_deliverables.visibility = View.VISIBLE
                        emptyTxtView1.visibility = View.GONE
                        recyclerAdapter = RecyclerAdapterDeliverableUpload(baseContext, responseBody, this@StudentComakershipActivity)
                        recyclerAdapter.notifyDataSetChanged()
                        recyclerview_deliverables.adapter = recyclerAdapter
                    }
                }
            }
            override fun onFailure(call: Call<List<Deliverable>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
            }
        })
    }

    private fun getComakerShip(token: String, comakershipId: Int) {
        var programs = ""
        val call = fetchApi().getComakership("Bearer "+token, comakershipId)
        call.enqueue(object : Callback<Comakership> {

            override fun onResponse(
                call: Call<Comakership>,
                response: Response<Comakership>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@StudentComakershipActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    val responseBody = response.body()

                    if(responseBody!!.status!!.id == 3) {
                        startNScrollV.isVisible = false
                        endNScrollV.isVisible = true
                        toolbarTitle.text = "Project Completed!"
                        if(responseBody.bonus) {
                            rewardValue.text = "bonus!"
                        } else if(responseBody.credits) {
                            rewardValue.text = "credits!"
                        } else {
                            rewardText.text = ""
                        }
                    }else {
                        getComakershipDeliverables(token, comakershipId)
                        toolbarTitle.text = responseBody!!.name
                        when (responseBody.status!!.id) {
                            1 -> {
                                statusValue.text = responseBody.status!!.name
                                statusValue.setTextColor(ContextCompat.getColor(this@StudentComakershipActivity, R.color.status_red))
                            }
                            2 -> {
                                statusValue.text = responseBody.status!!.name
                                statusValue.setTextColor(ContextCompat.getColor(this@StudentComakershipActivity, R.color.status_orange))
                            }
                            3 -> {
                                statusValue.text = responseBody.status!!.name
                                statusValue.setTextColor(ContextCompat.getColor(this@StudentComakershipActivity, R.color.primary_green))
                                reviewTitle.isVisible = true
                                reviewCommentTitle.isVisible = true
                                reviewComment.isVisible = true
                                ratingbar.isVisible = true
                                submitBtn.isVisible = true
                                reviewTxtinputlayout.isVisible = true

                                submitBtn.setOnClickListener{

                                    var review = PostReview(responseBody.company!!.id!!, tokenManager.getUserId(), ratingbar.rating.toInt(), reviewComment.text.toString(), true)
                                    val call = fetchApi().reviewCompany(review, "Bearer "+token)
                                    call.enqueue(object : Callback<Void> {
                                        override fun onResponse(
                                            call: Call<Void>,
                                            response: Response<Void>
                                        ) {
                                            if(response.code() == 401) {
                                                tokenManager.clearJwtToken()
                                                val intent = Intent(this@StudentComakershipActivity, LoginActivity::class.java)
                                                finish()
                                                startActivity(intent)
                                            }
                                            if (response.isSuccessful) {
                                                Toast.makeText(this@StudentComakershipActivity, "The review has been submitted!", Toast.LENGTH_SHORT).show()
                                            }
                                        }
                                        override fun onFailure(call: Call<Void>, t: Throwable) {
                                            Log.e("HTTP", "Could not fetch data", t)
                                        }
                                    })
                                }
                            }
                        }
                        responseBody.programs.forEach{
                            programs += " "+it.name.toString()+" | "
                        }
                        programValue.text = programs
                        titleValue.text = responseBody.name
                        descriptionValue.text = responseBody.description
                        responseBody.skills.forEach {
                            addChipToGroup(it.name, skillChipGroup)
                        }
                        val call = fetchApi().getTeam("Bearer "+token, comakershipId) //Todo Students is empty. Cannot request specific team.
                        call.enqueue(object : Callback<SpecificTeam> {

                            override fun onResponse(
                                call: Call<SpecificTeam>,
                                response: Response<SpecificTeam>
                            ) {
                                if(response.code() == 401) {
                                    tokenManager.clearJwtToken()
                                    val intent = Intent(this@StudentComakershipActivity, LoginActivity::class.java)
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
                                        recyclerAdapterTeamMembers = RecyclerAdapterTeamMembers(baseContext, responseBody.members, this@StudentComakershipActivity)
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
            }
            override fun onFailure(call: Call<Comakership>, t: Throwable) {
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

    override fun onTeamMemberClick(position: Int) {
    }

    override fun onDeliverableClick(position: Int) {
        if(responseBody[position].finished == false) {
            val intent = Intent(
                this,
                StudentComakershipDeliverableAlertDialogActivity::class.java
            )

            intent.putExtra("chosenComakershipId", comakershipString.toInt())
            this.finish()
            this.startActivity(intent)
        }
    }
}