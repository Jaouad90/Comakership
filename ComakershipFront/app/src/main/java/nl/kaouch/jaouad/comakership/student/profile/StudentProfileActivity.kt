package nl.kaouch.jaouad.comakership.student.profile

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.util.Patterns
import android.view.LayoutInflater
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.core.view.forEach
import androidx.core.view.isVisible
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.textfield.TextInputEditText
import com.google.android.material.textfield.TextInputLayout
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.requests.PostUpdateStudentProfile
import nl.kaouch.jaouad.comakership.models.responses.StudentUser
import nl.kaouch.jaouad.comakership.student.comakership.ComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.student.dashboard.StudentDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.team.TeamDashboardActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class StudentProfileActivity : AppCompatActivity() {

    private lateinit var tokenManager: TokenManager
    private lateinit var updateProfileBtn: Button
    private lateinit var addLinkBtn: ImageView
    private lateinit var student_edit_btn: ImageView
    private lateinit var student_nickname_value: TextView
    private lateinit var student_about_value: TextView
    private lateinit var student_name_value: TextView
    private lateinit var student_name_edittext: TextInputEditText
    private lateinit var student_nickname_edittext: TextInputEditText
    private lateinit var student_about_edittext: TextInputEditText
    private lateinit var student_link_edittext: TextInputEditText
    private lateinit var student_name_txtinputlayout: TextInputLayout
    private lateinit var student_nickname_txtinputlayout: TextInputLayout
    private lateinit var student_about_txtinputlayout: TextInputLayout
    private lateinit var student_links_txtinputlayout: TextInputLayout
    private lateinit var constraint_layout: ConstraintLayout
    private lateinit var container: LinearLayout
    private lateinit var responseBody: StudentUser
    private lateinit var linkList: List<String>
    private lateinit var logout_button: ImageView

    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            validateInput()
        }
    }

    private fun checkEditTextEmpty(editText: TextInputEditText): Boolean {
        if(editText.text!!.isEmpty()) {
            return true
        }
        return false
    }

    private fun validateInput() {
        if(Patterns.WEB_URL.matcher(student_name_edittext.text).matches() && student_name_edittext.text!!.length <= 50 || student_name_edittext.text!!.isEmpty()) {

        } else {
            updateProfileBtn.isEnabled = false
            student_name_edittext.error = "Enter a First and last name (ex. John Doe). \nThat has a max size of 50 characters."
        }

        if( Patterns.WEB_URL.matcher(student_link_edittext.text).matches() && student_link_edittext.text.toString().isNotEmpty()) {
            addLinkBtn.isEnabled = true
        } else {
            addLinkBtn.isEnabled = false
            student_link_edittext.error = "Enter a valid URL!!"
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_student_profile)

        tokenManager = TokenManager(applicationContext)
        student_nickname_value = findViewById(R.id.student_nickname_value)
        student_about_value = findViewById(R.id.student_about_value)
        student_name_value = findViewById(R.id.student_name_value)
        student_nickname_edittext = findViewById(R.id.student_nickname_edittext)
        student_name_edittext = findViewById(R.id.student_name_edittext)
        student_about_edittext = findViewById(R.id.student_about_edittext)
        student_link_edittext = findViewById(R.id.student_links_edittext)
        student_name_txtinputlayout = findViewById(R.id.student_name_txtinputlayout)
        student_nickname_txtinputlayout = findViewById(R.id.student_nickname_txtinputlayout)
        student_about_txtinputlayout = findViewById(R.id.student_about_txtinputlayout)
        student_links_txtinputlayout = findViewById(R.id.student_links_txtinputlayout)
        constraint_layout = findViewById(R.id.contraint_layout)
        container = findViewById(R.id.container)
        student_edit_btn = findViewById(R.id.join_team_btn)
        addLinkBtn = findViewById(R.id.add_link_btn)
        updateProfileBtn = findViewById(R.id.student_profile_save_btn)

        logout_button = findViewById(R.id.logout_button)

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@StudentProfileActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent)
        }

        student_name_edittext.addTextChangedListener(mTextWatcher)
        student_link_edittext.addTextChangedListener(mTextWatcher)
        linkList = emptyList()

        getStudentUser(tokenManager.getToken(), tokenManager.getUserId())

        validateInput()

        addLinkBtn.setOnClickListener {

            addLinkToGroupAsTxtViews(checkURL(student_link_edittext.text.toString()))
            student_link_edittext.text!!.clear()
        }

        student_edit_btn.setOnClickListener {
            if (student_nickname_txtinputlayout.isVisible) {
                student_nickname_value.isVisible = true
                student_about_value.isVisible = true
                student_name_value.isVisible = true

                student_name_txtinputlayout.isVisible = false
                student_nickname_txtinputlayout.isVisible = false
                student_about_txtinputlayout.isVisible = false
                student_links_txtinputlayout.isVisible = false
                addLinkBtn.isVisible = false
                updateProfileBtn.isEnabled = false
                container.forEach {
                    it.findViewById<ImageView>(R.id.remove_link_btn).isVisible = false
                }
            } else{
                student_nickname_value.isVisible = false
                student_about_value.isVisible = false
                student_name_value.isVisible = false

                student_name_txtinputlayout.isVisible = true
                student_nickname_txtinputlayout.isVisible = true
                student_about_txtinputlayout.isVisible = true
                student_links_txtinputlayout.isVisible = true
                addLinkBtn.isVisible = true
                addLinkBtn.isEnabled = false
                updateProfileBtn.isEnabled = true
                container.forEach {
                    it.findViewById<ImageView>(R.id.remove_link_btn).isVisible = true
                }
            }
        }

        updateProfileBtn.setOnClickListener {

            val s1 = when {
                checkEditTextEmpty(student_name_edittext) -> responseBody.name
                else -> student_name_edittext.text.toString()
            }

            val s2 = when {
                checkEditTextEmpty(student_nickname_edittext) -> responseBody.nickname
                else -> student_nickname_edittext.text.toString()
            }

            val s3 = when {
                checkEditTextEmpty(student_about_edittext) -> responseBody.about
                else -> student_about_edittext.text.toString()
            }

            val s4 = linkList

            var student = PostUpdateStudentProfile(
                    s1,
                    s3,
                    s4,
                    s2
            )

            setStudentUser(tokenManager.getToken(), student)
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_profile
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@StudentProfileActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@StudentProfileActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@StudentProfileActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@StudentProfileActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@StudentProfileActivity,
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

    private fun checkURL(link: String): String {
        if (!link.startsWith("http://") && !link.startsWith("https://")) {
            return "https://$link"
        } else {
            return link
        }
    }

    private fun addLinkToGroupAsTxtViews(link: String) {
        linkList += listOf(link)

        val layoutInflater = baseContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val addView = layoutInflater.inflate(R.layout.row, null)

        addView.findViewById<TextView>(R.id.linkText).text = link
        addView.findViewById<TextView>(R.id.linkText).setOnClickListener() {
            startActivity(Intent(Intent.ACTION_VIEW, Uri.parse(link)))
        }

        if (student_nickname_txtinputlayout.isVisible) {
            addView.findViewById<ImageView>(R.id.remove_link_btn).isVisible = true
        }
        addView.findViewById<ImageView>(R.id.remove_link_btn).setOnClickListener {
            container.removeView(addView)
            var linkText = addView.findViewById<TextView>(R.id.linkText).text.toString()
            var mutableLinkList = linkList.toMutableList()
            mutableLinkList.remove(linkText)
            linkList = mutableLinkList.toList()
        }

        container.addView(addView)
    }

    private fun setStudentUser(token: String, student: PostUpdateStudentProfile) {
        val call = fetchApi().setStudentInfo("Bearer " + token, student)
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                    call: Call<Void>,
                    response: Response<Void>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@StudentProfileActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    Toast.makeText(
                            this@StudentProfileActivity,
                            "Student profile updated succesfully!!",
                            Toast.LENGTH_SHORT
                    ).show()
                    finish()
                    startActivity(getIntent())
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
            }
        })
    }

    private fun getStudentUser(token: String, id: Int) {
        val call = fetchApi().getStudentUser("Bearer " + token, id)
        call.enqueue(object : Callback<StudentUser> {

            override fun onResponse(
                    call: Call<StudentUser>,
                    response: Response<StudentUser>
            ) {
                if(response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(this@StudentProfileActivity, LoginActivity::class.java)
                    finish()
                    startActivity(intent)
                }
                if (response.isSuccessful) {
                    responseBody = response.body()!!
                    student_nickname_value.text = responseBody.nickname
                    student_about_value.text = responseBody.about
                    student_name_value.text = responseBody.name
                    for (link in responseBody.links) {
                        addLinkToGroupAsTxtViews(checkURL(link))
                    }
                }
            }

            override fun onFailure(call: Call<StudentUser>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
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