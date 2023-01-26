package nl.kaouch.jaouad.comakership.register.company

import android.content.Intent
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.google.android.material.textfield.TextInputEditText
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.models.Company
import nl.kaouch.jaouad.comakership.models.responses.CompanyUser
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.login.LoginActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class RegisterCompanyPrimaryUserActivity : AppCompatActivity() {

    private lateinit var company: Company
    private lateinit var nameET: TextInputEditText
    private lateinit var emailET: TextInputEditText
    private lateinit var passwordET: TextInputEditText
    private lateinit var repeatPasswordET: TextInputEditText
    private lateinit var createBtn: Button
    private lateinit var toolbar_back_button: ImageView

    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            checkValidation()
        }
    }

    private fun checkValidation() {
        var s1 = ""
        var s2 = ""
        var s3 = ""
        var s4 = ""

        // Regex checks if a string contains only alphabet and space
        if(nameET.text!!.matches(Regex("^[آ-یA-z]{2,}( [آ-یA-z]{2,})+([آ-یA-z]|[ ]?)\$")) && nameET.text!!.length <= 50) {
            s1 = nameET.text.toString()
        } else {
            nameET.setError("Enter a First and last name (ex. John Doe). \nThat has a max size of 50 characters.")
        }

        // General Email Regex (RFC 5322 Official Standard)
        if(emailET.text!!.matches(Regex("(?:[a-z0-9!#\$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#\$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"))) {
            s2 = emailET.text.toString()
        } else {
            emailET.setError("You need to enter an existing email. Without uppercase characters!")
        }

//        Secure Password Regex
//        Password must contain at least one digit [0-9].
//        Password must contain at least one lowercase Latin character [a-z].
//        Password must contain at least one uppercase Latin character [A-Z].
//        Password must contain at least one special character like ! @ # & ( ).
//        Password must contain a length of at least 8 characters and a maximum of 20 characters.
        if(passwordET.text!!.matches(Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#&()–[{}]:;',?/*~$^+=<>]).{8,20}$"))) {
            s3 = passwordET.text.toString()
        } else {
            passwordET.setError("Password must contain at least one digit [0-9] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one special character like ! @ # & ( ) \n Password must contain a length of at least 8 characters and a maximum of 20 characters")
        }

        if(repeatPasswordET.text.toString() == s3) {
            s4 = repeatPasswordET.text.toString()
        } else {
            repeatPasswordET.setError("The passwords don't match!")
        }


        createBtn = findViewById(R.id.createBtn)

//        Check if inputFields are empty
        if (s1 == "" || s2 == "" || s3 == "" || s4 == "") {
             createBtn.isEnabled = false
        } else if (s4 == s3){
            createBtn.isEnabled = true
        } else {
            repeatPasswordET.setError("The passwords don't match!")
        }

    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_register_company_primary_user)

        company = ((intent.getSerializableExtra("companySerializable") as Company?)!!)
        nameET = findViewById(R.id.nameET)
        emailET = findViewById(R.id.emailET)
        passwordET = findViewById(R.id.passwordET)
        repeatPasswordET = findViewById(R.id.repeatPasswordET)
        var signinTxtview: TextView = findViewById(R.id.signinAnswerTXT)

        nameET.addTextChangedListener(mTextWatcher)
        emailET.addTextChangedListener(mTextWatcher)
        passwordET.addTextChangedListener(mTextWatcher)
        repeatPasswordET.addTextChangedListener(mTextWatcher)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@RegisterCompanyPrimaryUserActivity, RegisterCompanyActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        checkValidation()

        createBtn.setOnClickListener {
            register(company)
        }

        signinTxtview.setOnClickListener(View.OnClickListener {
            val mainIntent = Intent(
                    this@RegisterCompanyPrimaryUserActivity,
                    LoginActivity::class.java
            )
            startActivity(mainIntent)
        })

    }

    private fun register(company: Company) {

        company.CompanyUser = CompanyUser(
                null,
                nameET.text.toString(),
                emailET.text.toString(),
                repeatPasswordET.text.toString(),
                null,
                null,
                null
        )

        val call = fetchApi().registerCompany(company)
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
            ) {
                if (response.isSuccessful) {
                    println(response.body())
                    Toast.makeText(this@RegisterCompanyPrimaryUserActivity, "Company account succesfully created!!", Toast.LENGTH_SHORT).show()
                    val mainIntent = Intent(this@RegisterCompanyPrimaryUserActivity, LoginActivity::class.java)
                    startActivity(mainIntent)

                } else {
                    if (response.code() == 409) {
                        Toast.makeText(this@RegisterCompanyPrimaryUserActivity, "This company already exists!!", Toast.LENGTH_SHORT).show()
                    }
                    else {
                        Toast.makeText(this@RegisterCompanyPrimaryUserActivity, "There went something wrong!!", Toast.LENGTH_SHORT).show()
                    }
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
                Toast.makeText(this@RegisterCompanyPrimaryUserActivity, "Check the internet connection!", Toast.LENGTH_SHORT).show()

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