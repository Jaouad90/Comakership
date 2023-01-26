package nl.kaouch.jaouad.comakership.login

import android.content.Intent
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.models.requests.LoginRequest
import nl.kaouch.jaouad.comakership.models.responses.LoginResponse
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class LoginActivity : AppCompatActivity() {

    private lateinit var emailET: EditText
    private lateinit var passwordET:EditText
    private lateinit var signinBtn: Button
    private lateinit var tokenManager: TokenManager

    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {

            checkValidation()
        }
    }

    private fun checkValidation() {

        val s1: String = emailET.getText().toString()
        val s2: String = passwordET.getText().toString()
        signinBtn = findViewById(R.id.createBtn)
        if (s1 == "" || s2 == "") {
            emailET.setCompoundDrawablesWithIntrinsicBounds(R.drawable.custom_login_email_icon_focus_state, 0, 0, 0)
            passwordET.setCompoundDrawablesWithIntrinsicBounds(R.drawable.custom_login_password_icon_focus_state, 0, 0, 0)
            signinBtn.isEnabled = false
        } else {
            emailET.setCompoundDrawablesWithIntrinsicBounds(R.drawable.custom_login_email_icon_enable_state, 0, 0, 0)
            passwordET.setCompoundDrawablesWithIntrinsicBounds(R.drawable.custom_login_password_icon_enable_state, 0, 0, 0)
            signinBtn.isEnabled = true
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        tokenManager = TokenManager(applicationContext)

        emailET = findViewById(R.id.nameET)
        passwordET = findViewById(R.id.emailET)
        var registerTxtview: TextView = findViewById(R.id.registerAnswerTXT)


        emailET.addTextChangedListener(mTextWatcher)
        passwordET.addTextChangedListener(mTextWatcher)
        checkValidation()

        signinBtn.setOnClickListener {
            var loginRequest = LoginRequest(emailET.text.toString(), passwordET.text.toString())
            login(loginRequest)
        }

        registerTxtview.setOnClickListener(View.OnClickListener {
            val mainIntent = Intent(this@LoginActivity, MainActivity::class.java)
            startActivity(mainIntent)
        })
    }

    private fun login(credentials: LoginRequest) {
        val call = fetchApi().login(credentials)
        call.enqueue(object : Callback<LoginResponse> {

            override fun onResponse(
                call: Call<LoginResponse>,
                response: Response<LoginResponse>
            ) {
                if (response.isSuccessful) {

                    if(response.body()!!.UserType == "CompanyUser") {
                        println("CompanyUser : \n"+response.body()!!.Token)
                        Toast.makeText(this@LoginActivity, "Company account logged in succesfully!!", Toast.LENGTH_SHORT).show()
                        val mainIntent = Intent(this@LoginActivity, CompanyDashboardActivity::class.java)
                        tokenManager.createSession(response.body()!!.UserId, response.body()!!.Token)
                        startActivity(mainIntent)
                    } else if (response.body()!!.UserType == "StudentUser") {
                        println("StudentUser : \n"+response.body()!!.Token)
                        Toast.makeText(this@LoginActivity, "Student account logged in succesfully!!", Toast.LENGTH_SHORT).show()
                        val mainIntent = Intent(this@LoginActivity, StudentDashboardActivity::class.java)
                        tokenManager.createSession(response.body()!!.UserId, response.body()!!.Token)
                        startActivity(mainIntent)
                    }
                } else {
                    if (response.code() == 401) {
                        Toast.makeText(this@LoginActivity, "The account credentials are wrong!!", Toast.LENGTH_SHORT).show()
                    }
                    else {
                        Toast.makeText(this@LoginActivity, "There went something wrong!!", Toast.LENGTH_SHORT).show()
                    }
                }
            }

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
                Toast.makeText(this@LoginActivity, "Check the internet connection!", Toast.LENGTH_SHORT).show()

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