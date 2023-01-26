package nl.kaouch.jaouad.comakership.register.student

import android.content.Intent
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.View
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import com.google.android.material.textfield.TextInputEditText
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.models.Program
import nl.kaouch.jaouad.comakership.models.Student
import nl.kaouch.jaouad.comakership.models.UniversityDomain
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.login.LoginActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import kotlin.properties.Delegates


class RegisterStudentActivity : AppCompatActivity(), AdapterView.OnItemSelectedListener {

    private lateinit var student: Student
    private lateinit var name_et: TextInputEditText
    private lateinit var programs_dropdown: Spinner
    private lateinit var dropdownTxt: TextView
    private lateinit var email_et: TextInputEditText
    private lateinit var nickname_et: TextInputEditText
    private lateinit var password_et: TextInputEditText
    private lateinit var repeat_password_et: TextInputEditText
    private lateinit var createBtn: Button
    private lateinit var programIds: ArrayList<Int>
    private lateinit var programNames: ArrayList<String>
    private var chosenProgramId by Delegates.notNull<Int>()
    private lateinit var universityDomains: List<UniversityDomain>
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
        var s5 = ""

        // Regex checks if a string contains only alphabet and space
        if(name_et.text!!.matches(Regex("^[آ-یA-z]{2,}( [آ-یA-z]{2,})+([آ-یA-z]|[ ]?)\$")) && name_et.text!!.length <= 50) {
            s1 = name_et.text.toString()
        } else {
            name_et.error = "Enter a First and last name (ex. John Doe). \nThat has a max size of 50 characters."
        }

        // General Email Regex (RFC 5322 Official Standard)
        if(email_et.text!!.matches(Regex("(?:[a-z0-9!#\$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#\$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"))) {
            for(domain in universityDomains) {
                val index = email_et.text.toString().indexOf('@')

                val chosenDomain: String? = if (index == -1) null else email_et.text.toString().substring(index)
                if(chosenDomain.equals(domain.domain)) {
                    email_et.error = null
                    s2 = email_et.text.toString()
                    break
                } else {
                    email_et.error = "The domain doesnt exist!!"
                }
            }
        } else {
            email_et.error = "You need to enter an existing email. Without uppercase characters!"
        }

//        The nickname should not contain any special characters
        if(nickname_et.text!!.matches(Regex("^[^0-9]\\w+\$"))) {
            s5 = nickname_et.text.toString()
        } else {
            nickname_et.error = "The nickname should not contain any special characters!!"
        }

//        Secure Password Regex
//        Password must contain at least one digit [0-9].
//        Password must contain at least one lowercase Latin character [a-z].
//        Password must contain at least one uppercase Latin character [A-Z].
//        Password must contain at least one special character like ! @ # & ( ).
//        Password must contain a length of at least 8 characters and a maximum of 20 characters.
        if(password_et.text!!.matches(Regex("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#&()–[{}]:;',?/*~$^+=<>]).{8,20}$"))) {
            s3 = password_et.text.toString()
        } else {
            password_et.error = "Password must contain at least one digit [0-9] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one uppercase Latin character [A-Z] \n Password must contain at least one special character like ! @ # & ( ) \n Password must contain a length of at least 8 characters and a maximum of 20 characters"
        }

//        Secure Password Regex
        if(repeat_password_et.text.toString() == s3) {
            s4 = repeat_password_et.text.toString()
        } else {
            repeat_password_et.error = "The passwords don't match!"
        }


        createBtn = findViewById(R.id.create_btn)

//        Check if inputFields are empty
        if (s1 == "" || s2 == "" || s3 == "" || s4 == "" || s5 == "" || chosenProgramId == 0) {
            createBtn.isEnabled = false
        } else if (s4 == s3){
            createBtn.isEnabled = true
        } else {
            repeat_password_et.error = "The passwords don't match!"
        }

    }

    // Append ArrayList to ArrayList
    // The goal is to add an empty object at beginning of the array to display in the dropdown
    fun appendArrayList(arr: ArrayList<Program>?, element: ArrayList<Program>): ArrayList<Program> {
        val list: MutableList<Program> = arr!!.toMutableList()
        for(program in element) {
            list.add(program)
        }
        return list as ArrayList<Program>
    }

    private fun getDomains() {
        val call = fetchApi().getDomains()
        call.enqueue(object : Callback<List<UniversityDomain>> {

            override fun onResponse(
                    call: Call<List<UniversityDomain>>,
                    response: Response<List<UniversityDomain>>
            ) {
                if (response.isSuccessful) {
                    universityDomains = response.body()!!
                } else {
                    Toast.makeText(
                            this@RegisterStudentActivity,
                            "There went something wrong!!",
                            Toast.LENGTH_SHORT
                    ).show()
                }
            }

            override fun onFailure(call: Call<List<UniversityDomain>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(
                        this@RegisterStudentActivity,
                        "Check the internet connection!",
                        Toast.LENGTH_SHORT
                ).show()

            }
        })
    }


    private fun getPrograms() {
        val call = fetchApi().getPrograms()
        call.enqueue(object : Callback<ArrayList<Program>> {

            override fun onResponse(
                    call: Call<ArrayList<Program>>,
                    response: Response<ArrayList<Program>>
            ) {
                if (response.isSuccessful) {
                    var programs: ArrayList<Program>  = arrayListOf(Program(0, " "))
                    programs = appendArrayList(programs, response.body()!!)

                    for (program in programs) {
                        programNames.add(program.name!!)
                        programIds.add(program.id)
                    }

                    var adapter = ArrayAdapter(this@RegisterStudentActivity, android.R.layout.simple_spinner_item, programNames)
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                    programs_dropdown.adapter = adapter
                    programs_dropdown.onItemSelectedListener = this@RegisterStudentActivity
                } else {
                    Toast.makeText(
                            this@RegisterStudentActivity,
                            "There went something wrong!!",
                            Toast.LENGTH_SHORT
                    ).show()
                }
            }

            override fun onFailure(call: Call<ArrayList<Program>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(
                        this@RegisterStudentActivity,
                        "Check the internet connection!",
                        Toast.LENGTH_SHORT
                ).show()
            }
        })
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_register_student)
        getDomains()
        programIds = arrayListOf()
        programNames = arrayListOf()
        getPrograms()
        name_et = findViewById(R.id.name_et)
        programs_dropdown = findViewById(R.id.programs_dropdown)
        dropdownTxt = findViewById(R.id.spinner_message)
        email_et = findViewById(R.id.email_et)
        nickname_et = findViewById(R.id.nickname_et)
        password_et = findViewById(R.id.password_et)
        repeat_password_et = findViewById(R.id.repeat_password_et)
        var signinTxtview: TextView = findViewById(R.id.signinAnswerTXT)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@RegisterStudentActivity, MainActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        name_et.addTextChangedListener(mTextWatcher)
        email_et.addTextChangedListener(mTextWatcher)
        nickname_et.addTextChangedListener(mTextWatcher)
        password_et.addTextChangedListener(mTextWatcher)
        repeat_password_et.addTextChangedListener(mTextWatcher)

        checkValidation()

        createBtn.setOnClickListener {
            register()
        }

        signinTxtview.setOnClickListener(View.OnClickListener {
            val mainIntent = Intent(
                this@RegisterStudentActivity,
                LoginActivity::class.java
            )
            startActivity(mainIntent)
        })
    }

    private fun register() {
        var name =  name_et.text.toString().split(" ")
        student = Student(
            name[0],
            name[1],
            email_et.text.toString(),
            repeat_password_et.text.toString(),
            chosenProgramId,
            nickname_et.text.toString()
        )

        val call = fetchApi().registerStudent(student)
        call.enqueue(object : Callback<Void> {

            override fun onResponse(
                call: Call<Void>,
                response: Response<Void>
            ) {
                if (response.isSuccessful) {
                    println(response.body())
                    Toast.makeText(
                        this@RegisterStudentActivity,
                        "Student account succesfully created!!",
                        Toast.LENGTH_SHORT
                    ).show()
                    val mainIntent = Intent(
                        this@RegisterStudentActivity,
                        LoginActivity::class.java
                    )
                    startActivity(mainIntent)

                } else {
                    if (response.code() == 409) {
                        Toast.makeText(
                            this@RegisterStudentActivity,
                            "This student already exists!!",
                            Toast.LENGTH_SHORT
                        ).show()
                    } else {
                        Toast.makeText(
                            this@RegisterStudentActivity,
                            "There went something wrong!!",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(
                    this@RegisterStudentActivity,
                    "Check the internet connection!",
                    Toast.LENGTH_SHORT
                ).show()

            }
        })
    }
    override fun onItemSelected(p0: AdapterView<*>?, p1: View?, p2: Int, p3: Long) {
        chosenProgramId = programIds[p2]
        if (chosenProgramId == 0) {
            createBtn.isEnabled = false
        }
//        Toast.makeText(this@RegisterStudentActivity, "Selected program: "+programIds[p2] ,Toast.LENGTH_SHORT).show()
    }

    override fun onNothingSelected(p0: AdapterView<*>?) {
        createBtn.isEnabled = false
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