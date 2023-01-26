package nl.kaouch.jaouad.comakership.student.comakership

import android.app.AlertDialog
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.PrivateTeam
import nl.kaouch.jaouad.comakership.models.responses.SpecificTeam
import nl.kaouch.jaouad.comakership.models.responses.StudentUser
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import kotlin.properties.Delegates


class SearchComakershipAlertDialogActivity : AppCompatActivity(),
    AdapterView.OnItemSelectedListener {

    private lateinit var spinnerTeams: Spinner
    private lateinit var confirmBtn: Button
    private lateinit var tokenManager: TokenManager
    private lateinit var responseBodyTeams: List<PrivateTeam>
    private var chosenTeamId by Delegates.notNull<Int>()
    private lateinit var toolbarBackButton: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {

        tokenManager = TokenManager(applicationContext)
        responseBodyTeams = emptyList()

        displayAlert()
        super.onCreate(savedInstanceState)
    }

    private fun joinComakership() {
        var teamNames = emptyList<String>()
        responseBodyTeams.forEach {
            teamNames += it.name
        }
        var adapter = ArrayAdapter(
            this@SearchComakershipAlertDialogActivity,
            android.R.layout.simple_spinner_item,
            teamNames
        )
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        spinnerTeams.adapter = adapter
        spinnerTeams.onItemSelectedListener = this@SearchComakershipAlertDialogActivity

        confirmBtn.setOnClickListener(View.OnClickListener {
            var chosenComakershipId = intent.getIntExtra("chosenComakershipId", -1)
            val call = fetchApi().applyComakershipAsTeam(
                "Bearer " + tokenManager.getToken(),
                chosenTeamId,
                chosenComakershipId
            )
            call.enqueue(object : Callback<Void> {

                override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
                ) {
                    if (response.code() == 401) {
                        tokenManager.clearJwtToken()
                        val intent = Intent(
                            this@SearchComakershipAlertDialogActivity,
                            LoginActivity::class.java
                        )
                        this@SearchComakershipAlertDialogActivity.startActivity(
                            intent
                        )
                    }
                    if (response.isSuccessful) {
                        Toast.makeText(
                            this@SearchComakershipAlertDialogActivity,
                            "The application has been successfully executed!!",
                            Toast.LENGTH_SHORT
                        ).show()

                        val intent = Intent(
                            this@SearchComakershipAlertDialogActivity,
                            ComakershipDashboardActivity::class.java
                        )
                        this@SearchComakershipAlertDialogActivity.finish()
                        this@SearchComakershipAlertDialogActivity.startActivity(
                            intent
                        )
                    } else {
                        Toast.makeText(
                            this@SearchComakershipAlertDialogActivity,
                            "You have already submitted a request!!",
                            Toast.LENGTH_SHORT
                        ).show()
                        val intent = Intent(
                            this@SearchComakershipAlertDialogActivity,
                            SearchComakershipActivity::class.java
                        )
                        this@SearchComakershipAlertDialogActivity.finish()
                        this@SearchComakershipAlertDialogActivity.startActivity(
                            intent
                        )
                    }
                }

                override fun onFailure(
                    call: Call<Void>,
                    t: Throwable
                ) {
                    Log.e(
                        "HTTP",
                        "Could not fetch data",
                        t
                    )
                    Toast.makeText(
                        this@SearchComakershipAlertDialogActivity,
                        "Check the internet connection!",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            })
        })
    }

    private fun displayAlert() {
        val alertDialogBuilder: AlertDialog.Builder = AlertDialog.Builder(this)
        val inflater: LayoutInflater = this.layoutInflater
        val view = inflater.inflate(R.layout.apply_comakership_team_dialog, null)

        spinnerTeams = view.findViewById(R.id.teams_dropdown)
        confirmBtn = view.findViewById(R.id.confirm_comakership_btn)
        toolbarBackButton = view.findViewById(R.id.toolbar_back_button)
        toolbarBackButton.setOnClickListener {
            val intent = Intent(this@SearchComakershipAlertDialogActivity, ComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }
        alertDialogBuilder.setView(view)
        val alertDialog: AlertDialog = alertDialogBuilder.create()
        alertDialog.show()

        val call = fetchApi().getStudentUser("Bearer " + tokenManager.getToken(), tokenManager.getUserId())
        call.enqueue(object : Callback<StudentUser> {

            override fun onResponse(
                call: Call<StudentUser>,
                response: Response<StudentUser>
            ) {
                if (response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(
                        this@SearchComakershipAlertDialogActivity,
                        LoginActivity::class.java
                    )
                    this@SearchComakershipAlertDialogActivity.finish()
                    this@SearchComakershipAlertDialogActivity.startActivity(intent)
                }
                if (response.isSuccessful) {
                    if (!response.body()!!.equals(null))
                        response.body()!!.linkedTeams.forEach {

                            val call =
                                fetchApi().getTeam("Bearer " + tokenManager.getToken(), it.teamId)
                            call.enqueue(object : Callback<SpecificTeam> {

                                override fun onResponse(
                                    call: Call<SpecificTeam>,
                                    response: Response<SpecificTeam>
                                ) {
                                    if (response.code() == 401) {
                                        tokenManager.clearJwtToken()
                                        val intent = Intent(
                                            this@SearchComakershipAlertDialogActivity,
                                            LoginActivity::class.java
                                        )
                                        this@SearchComakershipAlertDialogActivity.startActivity(
                                            intent
                                        )
                                    }
                                    if (response.isSuccessful) {
                                        var responseBodyTeam = response.body()
                                        if (responseBodyTeam != null)
                                            responseBodyTeams += PrivateTeam(
                                                responseBodyTeam.id,
                                                responseBodyTeam.name,
                                                responseBodyTeam.description,
                                                emptyList()
                                            )
                                        joinComakership()
                                    } else {
                                        Toast.makeText(
                                            this@SearchComakershipAlertDialogActivity,
                                            response.message(),
                                            Toast.LENGTH_SHORT
                                        ).show()
                                    }
                                }

                                override fun onFailure(
                                    call: Call<SpecificTeam>,
                                    t: Throwable
                                ) {
                                    Log.e(
                                        "HTTP",
                                        "Could not fetch data",
                                        t
                                    )
                                    Toast.makeText(
                                        this@SearchComakershipAlertDialogActivity,
                                        "Check the internet connection!",
                                        Toast.LENGTH_SHORT
                                    ).show()
                                }
                            })
                        }
                }
            }

            override fun onFailure(
                call: Call<StudentUser>,
                t: Throwable
            ) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(
                    this@SearchComakershipAlertDialogActivity,
                    "Check the internet connection!",
                    Toast.LENGTH_SHORT
                ).show()

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

    override fun onItemSelected(p0: AdapterView<*>?, p1: View?, p2: Int, p3: Long) {
        confirmBtn.isEnabled = true
        chosenTeamId = responseBodyTeams[p2].id
    }

    override fun onNothingSelected(p0: AdapterView<*>?) {
        confirmBtn.isEnabled = false
    }
}