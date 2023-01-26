package nl.kaouch.jaouad.comakership.company.comakerships

import android.app.AlertDialog
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.widget.*
import androidx.core.view.isVisible
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Deliverable
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import kotlin.properties.Delegates

class CompanyComakershipDeliverableAlertDialogActivity : AppCompatActivity(),
    AdapterView.OnItemSelectedListener {

    private lateinit var tokenManager: TokenManager
    private lateinit var toolbarBackButton: ImageView
    private lateinit var statusDropdown: Spinner
    private lateinit var deliverableStatus: Button
    private var isFinished by Delegates.notNull<Boolean>()
    private var listStatus = listOf<String>("Finished", "Not finished")

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        tokenManager = TokenManager(applicationContext)

        displayAlert()

        var adapter = ArrayAdapter(
            this,
            android.R.layout.simple_spinner_item,
            listStatus
        )
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        statusDropdown.adapter = adapter
        statusDropdown.onItemSelectedListener =
            this
    }

    private fun getDeliverable(chosenDeliverableId: Int) {
        val call = fetchApi().getDeliverable("Bearer "+tokenManager.getToken(), chosenDeliverableId)
        call.enqueue(object : Callback<Deliverable> {
            override fun onResponse(
                call: Call<Deliverable>,
                response: Response<Deliverable>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyComakershipDeliverableAlertDialogActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    var responseBody = response.body()

                    var deliverable = Deliverable(responseBody!!.id,responseBody.comakershipId,responseBody.name, isFinished ,responseBody.comakership)

                    val call = fetchApi().updateDeliverable("Bearer "+tokenManager.getToken(), chosenDeliverableId, deliverable)
                    call.enqueue(object : Callback<Void> {
                        override fun onResponse(
                            call: Call<Void>,
                            response: Response<Void>
                        ) {
                            if (response.isSuccessful) {
                                Toast.makeText(this@CompanyComakershipDeliverableAlertDialogActivity,
                                    "Status changed to $isFinished", Toast.LENGTH_SHORT).show()
                                val main = Intent(this@CompanyComakershipDeliverableAlertDialogActivity, CompanyComakershipDashboardActivity::class.java)
                                finish()
                                startActivity(main)
                            }
                        }
                        override fun onFailure(call: Call<Void>, t: Throwable) {
                            Log.e("HTTP", "Could not fetch data", t)
                        }
                    })
                }
            }
            override fun onFailure(call: Call<Deliverable>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
            }
        })
    }

    private fun displayAlert() {
        val alertDialogBuilder: AlertDialog.Builder = AlertDialog.Builder(this)
        val inflater: LayoutInflater = this.layoutInflater
        val view = inflater.inflate(R.layout.company_deliverable_upload_dialog, null)
        alertDialogBuilder.setView(view)

        toolbarBackButton = view.findViewById(R.id.toolbar_back_button)
        statusDropdown = view.findViewById(R.id.deliverable_status_dropdown)
        deliverableStatus = view.findViewById(R.id.submit_deliverable_status)

        toolbarBackButton.setOnClickListener {
            val intent = Intent(this@CompanyComakershipDeliverableAlertDialogActivity, CompanyComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        deliverableStatus.setOnClickListener{
            val chosenDeliverableId = intent.getIntExtra("chosenDeliverableId", 0)
            getDeliverable(chosenDeliverableId)
        }

        val alertDialog: AlertDialog = alertDialogBuilder.create()
        alertDialog.show()
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
        deliverableStatus.isVisible = true
        isFinished = listStatus[p2]=="Finished"
    }

    override fun onNothingSelected(p0: AdapterView<*>?) {
        deliverableStatus.isVisible = false
    }
}