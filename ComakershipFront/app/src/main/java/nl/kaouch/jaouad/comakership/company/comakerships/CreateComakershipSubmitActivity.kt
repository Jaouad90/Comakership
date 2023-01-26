package nl.kaouch.jaouad.comakership.company.comakerships

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.ImageView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.FragmentTransaction
import com.example.androidmaterialchips.ProgramEntryChipFragment
import com.example.androidmaterialchips.SkillEntryChipFragment
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.models.requests.PostCreateComakership
import nl.kaouch.jaouad.comakership.models.Skill
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class CreateComakershipSubmitActivity : AppCompatActivity(), ProgramEntryChipFragment.IProgramListener, SkillEntryChipFragment.ISkillListener {

    private lateinit var comakership: PostCreateComakership
    private lateinit var createBtn: Button
    private lateinit var tokenManager: TokenManager
    private lateinit var chosenProgramIds: ArrayList<Int>
    private lateinit var chosenSkillNames: ArrayList<Skill>
    private lateinit var toolbar_back_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_create_comakership_submit)

        chosenProgramIds = arrayListOf()
        chosenSkillNames = arrayListOf()
        createBtn = findViewById(R.id.create_team_btn)
        comakership = (intent.getSerializableExtra("PostCreateComakership") as PostCreateComakership)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@CreateComakershipSubmitActivity, CompanyComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        val programs: FragmentTransaction = supportFragmentManager.beginTransaction()
        programs.replace(R.id.programChipGroupPlaceholder, ProgramEntryChipFragment())
        programs.commit()

        val skills: FragmentTransaction = supportFragmentManager.beginTransaction()
        skills.replace(R.id.skillsChipGroupPlaceholder, SkillEntryChipFragment())
        skills.commit()

        createBtn.setOnClickListener {
            var freshComakership = PostCreateComakership(
                comakership.name,
                comakership.description,
                chosenSkillNames,
                chosenProgramIds,
                comakership.credits,
                comakership.bonus,
                comakership.deliverables,
                comakership.purchaseKey
            )
            comakership = freshComakership
            if(comakership.skills.size > 0 && comakership.programIds.size > 0) {
                tokenManager = TokenManager(applicationContext)
                createComakership(comakership, tokenManager.getToken())
            } else {
                Toast.makeText(this@CreateComakershipSubmitActivity, "Not all the fields have a value!", Toast.LENGTH_SHORT).show()
            }
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CreateComakershipSubmitActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CreateComakershipSubmitActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CreateComakershipSubmitActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CreateComakershipSubmitActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }

    private fun createComakership(comakership: PostCreateComakership, token: String) {
        val call = fetchApi().createComakership(comakership, "Bearer "+token)
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CreateComakershipSubmitActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    Toast.makeText(this@CreateComakershipSubmitActivity, "Comakership succesfully created!!", Toast.LENGTH_LONG).show()
                    val mainIntent = Intent(this@CreateComakershipSubmitActivity, CompanyComakershipDashboardActivity::class.java)
                    finish()
                    startActivity(mainIntent)
                } else {
                    Toast.makeText(this@CreateComakershipSubmitActivity, response.message(), Toast.LENGTH_SHORT).show()
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
                Toast.makeText(this@CreateComakershipSubmitActivity, "Check the internet connection!", Toast.LENGTH_SHORT).show()
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

    override fun setPrograms(chosenPrograms: ArrayList<Int>) {
        chosenProgramIds = chosenPrograms
    }

    override fun setSkills(chosenSkills: ArrayList<Skill>) {
        chosenSkillNames = chosenSkills
    }
}