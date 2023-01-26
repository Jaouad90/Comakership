package nl.kaouch.jaouad.comakership.company.profile

import android.content.Intent
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.constraintlayout.widget.ConstraintSet
import androidx.core.view.isVisible
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.textfield.TextInputEditText
import com.google.android.material.textfield.TextInputLayout
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.models.responses.CompanyUser
import nl.kaouch.jaouad.comakership.models.requests.PostAddCompanyUser
import nl.kaouch.jaouad.comakership.models.requests.PostChangePassword
import nl.kaouch.jaouad.comakership.R
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


class CompanyUserProfileActivity : AppCompatActivity() {

    private lateinit var constraint_layout: ConstraintLayout
    private lateinit var company_user_id: TextView
    private lateinit var company_user_name: TextView
    private lateinit var company_user_email: TextView
    private lateinit var company_user_add: TextView
    private lateinit var company_user_change_password_btn: Button
    private lateinit var company_user_add_btn: Button
    private lateinit var company_user_profile_save_btn: Button

    private lateinit var company_user_admin: ImageView
    private lateinit var edit_user_profile: ImageView
    private lateinit var toolbar_back_button: ImageView

    private lateinit var company_user_name_edit: EditText
    private lateinit var company_user_oldpassword_edit: TextInputEditText
    private lateinit var company_user_password_edit: TextInputEditText
    private lateinit var company_user_repeatpassword_edit: TextInputEditText
    private lateinit var company_user_oldpassword_txtinputlayout: TextInputLayout
    private lateinit var company_user_password_txtinputlayout: TextInputLayout
    private lateinit var company_user_repeat_password_textinputlayout: TextInputLayout

    private lateinit var company_newuser_email_txtinputlayout: TextInputLayout
    private lateinit var company_newuser_email_edittext: TextInputEditText
    private lateinit var company_newuser_admin_checkbox: CheckBox
    private lateinit var tokenManager: TokenManager
    private lateinit var companyUser: CompanyUser

    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            checkFieldsForEmptyValues()
        }
    }

    private val mTextWatcher1: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            validateEmail()
        }
    }

    private fun validateEmail() {
        var s4 = ""
        // General Email Regex (RFC 5322 Official Standard)
        if(company_newuser_email_edittext.text!!.matches(Regex("(?:[a-z0-9!#\$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#\$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"))) {
            s4 = company_newuser_email_edittext.text.toString()
        } else {
            company_newuser_email_edittext.setError("Choose another email address!!")
        }

        company_user_profile_save_btn.isEnabled = s4 != ""
    }

    private fun checkFieldsForEmptyValues() {
        var s1 = ""
        var s2 = ""
        var s3 = ""
//        Secure Password Regex
//        Password must contain at least one digit [0-9].
//        Password must contain at least one lowercase Latin character [a-z].
//        Password must contain at least one uppercase Latin character [A-Z].
//        Password must contain at least one special character like ! @ # & ( ).
//        Password must contain a length of at least 8 characters and a maximum of 20 characters.
        if(company_user_oldpassword_edit.text!!.matches(Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#&()–[{}]:;',?/*~$^+=<>]).{8,20}$"))) {
            s1 = company_user_oldpassword_edit.text.toString()
        } else {
            company_user_oldpassword_edit.setError("Password must contain at least one digit [0-9] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one special character like ! @ # & ( ) \n Password must contain a length of at least 8 characters and a maximum of 20 characters")
        }

        if(company_user_password_edit.text!!.matches(Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#&()–[{}]:;',?/*~$^+=<>]).{8,20}$"))) {
            s2 = company_user_password_edit.text.toString()
        } else {
            company_user_password_edit.setError("Password must contain at least one digit [0-9] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one special character like ! @ # & ( ) \n Password must contain a length of at least 8 characters and a maximum of 20 characters")
        }

        if(company_user_repeatpassword_edit.text.toString() == s2) {
            s3 = company_user_repeatpassword_edit.text.toString()
        } else {
            company_user_repeatpassword_edit.setError("The passwords don't match!")
        }

//        Check if inputFields are empty
        if(s1 == "" || s2 == "" || s3 == "") {
            company_user_profile_save_btn.isEnabled = false
        } else if (s2 == s3){
            company_user_profile_save_btn.isEnabled = true
        }else {
            company_user_repeatpassword_edit.setError("The passwords don't match!")
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_company_user_profile)

        companyUser = intent.getSerializableExtra("companyUser") as CompanyUser

        tokenManager = TokenManager(applicationContext)
        constraint_layout = findViewById(R.id.profile_constrain_scroll)
        company_user_id = findViewById(R.id.company_user_id_txtview_value)
        company_user_name = findViewById(R.id.company_user_name_txtview_value)
        company_user_email = findViewById(R.id.company_user_email_txtview_value)
        company_user_add = findViewById(R.id.company_user_add_txtview)
        company_user_change_password_btn = findViewById(R.id.company_user_profile_password_change_btn)
        company_user_add_btn = findViewById(R.id.company_user_add_btn)
        company_user_profile_save_btn = findViewById(R.id.company_user_profile_save_btn)
        company_user_admin = findViewById(R.id.admin_image)
        edit_user_profile = findViewById(R.id.join_team_btn)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        company_user_name_edit = findViewById(R.id.company_user_name_edittext)
        company_user_oldpassword_edit = findViewById(R.id.company_user_oldpassword_edittext)
        company_user_password_edit = findViewById(R.id.company_user_password_edittext)
        company_user_repeatpassword_edit = findViewById(R.id.company_user_repeat_password_edittext)
        company_newuser_email_edittext = findViewById(R.id.company_newuser_email_edittext)
        company_newuser_email_txtinputlayout = findViewById(R.id.company_newuser_email_txtinputlayout)
        company_user_oldpassword_txtinputlayout = findViewById(R.id.company_user_oldpassword_txtinputlayout)
        company_user_password_txtinputlayout = findViewById(R.id.company_user_password_txtinputlayout)
        company_user_repeat_password_textinputlayout = findViewById(R.id.company_user_repeat_password_textinputlayout)
        company_newuser_admin_checkbox = findViewById(R.id.company_newuser_admin_checkbox)

        company_user_oldpassword_edit.addTextChangedListener(mTextWatcher)
        company_user_password_edit.addTextChangedListener(mTextWatcher)
        company_user_repeatpassword_edit.addTextChangedListener(mTextWatcher)
        company_newuser_email_edittext.addTextChangedListener(mTextWatcher1)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@CompanyUserProfileActivity, CompanyProfileDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        getCompanyUser(tokenManager.getToken(), tokenManager.getUserId())

        edit_user_profile.setOnClickListener {
            if(!company_user_name_edit.isEnabled) {
                company_user_add.isVisible = true
                company_user_name_edit.isEnabled = true
                company_user_name_edit.isVisible = true
                company_user_change_password_btn.isEnabled = true
                company_user_change_password_btn.isVisible = true
                company_user_add_btn.isEnabled = true
                company_user_add_btn.isVisible = true
                company_user_profile_save_btn.isEnabled = true
                company_user_profile_save_btn.isVisible = true
                company_user_oldpassword_txtinputlayout.isEnabled = false
                company_user_oldpassword_txtinputlayout.isVisible = false
                company_user_password_txtinputlayout.isEnabled = false
                company_user_password_txtinputlayout.isVisible = false
                company_user_repeat_password_textinputlayout.isEnabled = false
                company_user_repeat_password_textinputlayout.isVisible = false
                company_newuser_email_txtinputlayout.isEnabled = false
                company_newuser_email_txtinputlayout.isVisible = false
                company_newuser_admin_checkbox.isEnabled = false
                company_newuser_admin_checkbox.isVisible = false
                val constraintSet = ConstraintSet()
                constraintSet.clone(constraint_layout)
                constraintSet.connect(
                    R.id.company_user_profile_save_btn,
                    ConstraintSet.TOP,
                    R.id.company_user_add_btn,
                    ConstraintSet.BOTTOM,
                    50
                )
                constraintSet.applyTo(constraint_layout)

                val constraintSetEmail = ConstraintSet()
                constraintSetEmail.clone(constraint_layout)
                constraintSetEmail.connect(
                    R.id.company_user_profile_password_change_btn,
                    ConstraintSet.TOP,
                    R.id.company_user_email_edittext,
                    ConstraintSet.BOTTOM,
                    0
                )
                constraintSetEmail.applyTo(constraint_layout)

                val constraintSetCheckbox = ConstraintSet()
                constraintSetCheckbox.clone(constraint_layout)
                constraintSetCheckbox.connect(
                    R.id.company_newuser_email_txtinputlayout,
                    ConstraintSet.TOP,
                    R.id.company_newuser_admin_checkbox,
                    ConstraintSet.BOTTOM,
                    0
                )
                constraintSetCheckbox.applyTo(constraint_layout)
            } else {
                company_user_add.isVisible = true
                company_user_name_edit.isEnabled = false
                company_user_name_edit.isVisible = false
                company_user_change_password_btn.isEnabled = false
                company_user_add_btn.isEnabled = false
                company_user_profile_save_btn.isEnabled = false
                company_user_profile_save_btn.isVisible = false
                company_user_oldpassword_txtinputlayout.isEnabled = false
                company_user_oldpassword_txtinputlayout.isVisible = false
                company_user_password_txtinputlayout.isEnabled = false
                company_user_password_txtinputlayout.isVisible = false
                company_user_repeat_password_textinputlayout.isEnabled = false
                company_user_repeat_password_textinputlayout.isVisible = false
                company_newuser_email_txtinputlayout.isEnabled = false
                company_newuser_email_txtinputlayout.isVisible = false
                company_newuser_admin_checkbox.isEnabled = false
                company_newuser_admin_checkbox.isVisible = false
            }
        }

        company_user_add_btn.setOnClickListener {
            if (!company_newuser_email_edittext.isEnabled) {
                company_user_name_edit.isEnabled = false
                company_user_name_edit.isVisible = false
                company_user_oldpassword_txtinputlayout.isEnabled = false
                company_user_oldpassword_txtinputlayout.isVisible = false
                company_user_password_txtinputlayout.isEnabled = false
                company_user_password_txtinputlayout.isVisible = false
                company_user_repeat_password_textinputlayout.isEnabled = false
                company_user_repeat_password_textinputlayout.isVisible = false
                company_user_change_password_btn.isEnabled = false
                company_user_change_password_btn.isVisible = true
                company_user_add_btn.isEnabled = false
                company_user_add_btn.isVisible = false
                company_newuser_email_txtinputlayout.isEnabled = true
                company_newuser_email_txtinputlayout.isVisible = true
                company_newuser_email_edittext.isEnabled = true
                company_newuser_email_edittext.isVisible = true
                company_newuser_admin_checkbox.isEnabled = true
                company_newuser_admin_checkbox.isVisible = true

                val constraintSetEmail = ConstraintSet()
                constraintSetEmail.clone(constraint_layout)
                constraintSetEmail.connect(
                    R.id.company_newuser_email_txtinputlayout,
                    ConstraintSet.TOP,
                    R.id.company_user_profile_password_change_btn,
                    ConstraintSet.BOTTOM,
                    0
                )
                constraintSetEmail.applyTo(constraint_layout)

                val constraintSetCheckbox = ConstraintSet()
                constraintSetCheckbox.clone(constraint_layout)
                constraintSetCheckbox.connect(
                    R.id.company_newuser_admin_checkbox,
                    ConstraintSet.TOP,
                    R.id.company_newuser_email_txtinputlayout,
                    ConstraintSet.BOTTOM,
                    0
                )
                constraintSetCheckbox.applyTo(constraint_layout)

                val constraintSet = ConstraintSet()
                constraintSet.clone(constraint_layout)
                constraintSet.connect(
                    R.id.company_user_profile_save_btn,
                    ConstraintSet.TOP,
                    R.id.company_newuser_admin_checkbox,
                    ConstraintSet.BOTTOM,
                    50
                )
                constraintSet.applyTo(constraint_layout)
                validateEmail()
            }
        }

        company_user_change_password_btn.setOnClickListener {
            if (!company_user_oldpassword_edit.isEnabled) {
                company_user_name_edit.isEnabled = false
                company_user_name_edit.isVisible = false
                company_user_oldpassword_txtinputlayout.isEnabled = true
                company_user_oldpassword_txtinputlayout.isVisible = true
                company_user_password_txtinputlayout.isEnabled = true
                company_user_password_txtinputlayout.isVisible = true
                company_user_repeat_password_textinputlayout.isEnabled = true
                company_user_repeat_password_textinputlayout.isVisible = true
                company_user_change_password_btn.isEnabled = false
                company_user_change_password_btn.isVisible = false
                company_user_add_btn.isEnabled = false
                company_user_add_btn.isVisible = false
                company_newuser_email_edittext.isEnabled = false
                company_newuser_email_edittext.isVisible = false
                company_newuser_admin_checkbox.isEnabled = false
                company_newuser_admin_checkbox.isVisible = false
                company_user_add.isVisible = false

                val constraintSet = ConstraintSet()
                constraintSet.clone(constraint_layout)
                constraintSet.connect(
                    R.id.company_user_profile_save_btn,
                    ConstraintSet.TOP,
                    R.id.company_user_repeat_password_textinputlayout,
                    ConstraintSet.BOTTOM,
                    0
                )
                constraintSet.applyTo(constraint_layout)
                checkFieldsForEmptyValues()
            }
        }

        company_user_profile_save_btn.setOnClickListener {
            if(company_user_name_edit.isEnabled) {
                var companyUser = CompanyUser(
                    null,
                    company_user_name_edit.text.toString(),
                    null,
                    null,
                    null,
                    null,
                    null
                )
                setCompanyUserInfo(companyUser)
            } else if (company_user_oldpassword_edit.isEnabled){
                var postChangePassword = PostChangePassword(
                    company_user_oldpassword_edit.text.toString(),
                    company_user_password_edit.text.toString(),
                    company_user_repeatpassword_edit.text.toString()
                )
                setUserPassword(postChangePassword)
            } else if (company_newuser_email_edittext.isEnabled){
                createCompanyUser(PostAddCompanyUser(company_newuser_email_edittext.text.toString(), company_newuser_admin_checkbox.isChecked))
            }
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_profile
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CompanyUserProfileActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CompanyUserProfileActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CompanyUserProfileActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CompanyUserProfileActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }

    private fun createCompanyUser(postAddCompanyUser: PostAddCompanyUser) {
        val call = fetchApi().addCompanyUser(
            "Bearer " + tokenManager.getToken(),
            postAddCompanyUser,
            companyUser.company!!.id!!
        )
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyUserProfileActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    val responseBody = response.body()
                    println(responseBody)
                    Toast.makeText(
                        this@CompanyUserProfileActivity,
                        "User profile created succesfully!!",
                        Toast.LENGTH_SHORT
                    ).show()
                    finish()
                    startActivity(getIntent())
                } else {
                    Toast.makeText(
                        this@CompanyUserProfileActivity,
                        response.errorBody().toString(),
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
    }

    private fun setUserPassword(postChangePassword: PostChangePassword) {
        val call = fetchApi().setUserPassword(
            "Bearer " + tokenManager.getToken(),
            postChangePassword
        )
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyUserProfileActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    val responseBody = response.body()
                    println(responseBody)
                    Toast.makeText(
                        this@CompanyUserProfileActivity,
                        "User profile updated succesfully!!",
                        Toast.LENGTH_SHORT
                    ).show()
                    finish()
                    startActivity(getIntent())
                } else if (response.code() == 400) {

                    Toast.makeText(
                        this@CompanyUserProfileActivity,
                        response.errorBody()!!.string(),
                        Toast.LENGTH_LONG
                    ).show()
                    val intent = Intent(
                        this@CompanyUserProfileActivity,
                        CompanyUserProfileActivity::class.java
                    )
                    intent.putExtra("companyUser", companyUser)
                    finish()
                    startActivity(intent)
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
    }

    private fun setCompanyUserInfo(companyUser: CompanyUser) {
        val call = fetchApi().setCompanyUserInfo(
            "Bearer " + tokenManager.getToken(),
            companyUser
        )
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@CompanyUserProfileActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    val responseBody = response.body()
                    println(responseBody)
                    Toast.makeText(
                        this@CompanyUserProfileActivity,
                        "User profile updated succesfully!!",
                        Toast.LENGTH_SHORT
                    ).show()
                    finish()
                    startActivity(getIntent())
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t);
            }
        })
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
                    val intent = Intent(this@CompanyUserProfileActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    companyUser = response.body()!!
                    company_user_id.text = companyUser.id.toString()
                    company_user_name.text = companyUser.name
                    company_user_email.text = companyUser.email
                    if (companyUser.isCompanyAdmin!!) {
                        company_user_admin.isVisible = true
                    }

                }
            }

            override fun onFailure(call: Call<CompanyUser>, t: Throwable) {
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