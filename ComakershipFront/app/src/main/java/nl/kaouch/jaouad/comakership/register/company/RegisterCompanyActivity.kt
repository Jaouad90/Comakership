package nl.kaouch.jaouad.comakership.register.company

import android.content.Intent
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.ImageView
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.models.Company
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.login.LoginActivity

class RegisterCompanyActivity : AppCompatActivity() {

    private lateinit var streetET: EditText
    private lateinit var cityET: EditText
    private lateinit var zipCodeET: EditText
    private lateinit var descriptionET: EditText
    private lateinit var companyNameET: EditText
    private lateinit var continueBtn: Button
    private lateinit var company: Company
    private lateinit var toolbar_back_button: ImageView


    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            checkFieldsForEmptyValues()
        }
    }

    private fun checkValidation(): Boolean {
        //TODO Validate the input fields
        return true
    }

    private fun checkFieldsForEmptyValues() {

        val s1: String = streetET.getText().toString()
        val s2: String = cityET.getText().toString()
        val s3: String = zipCodeET.getText().toString()
//        val s4: String = descriptionET.getText().toString()
        val s5: String = companyNameET.getText().toString()
        continueBtn = findViewById(R.id.createBtn)

        if (s1 == "" || s2 == "" || s3 == "" || s5 == "") {
            continueBtn.isEnabled = false
        } else {
            if(checkValidation()) {
                continueBtn.isEnabled = true
            }
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_register_company)

        streetET = findViewById(R.id.street_et)
        cityET = findViewById(R.id.city_et)
        zipCodeET = findViewById(R.id.zipcode_et)
        descriptionET = findViewById(R.id.description_et)
        companyNameET = findViewById(R.id.companyName_et)
        var signinTxtview: TextView = findViewById(R.id.signinAnswerTXT)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@RegisterCompanyActivity, MainActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        streetET.addTextChangedListener(mTextWatcher)
        cityET.addTextChangedListener(mTextWatcher)
        zipCodeET.addTextChangedListener(mTextWatcher)
        descriptionET.addTextChangedListener(mTextWatcher)
        companyNameET.addTextChangedListener(mTextWatcher)

        checkFieldsForEmptyValues()

        continueBtn.setOnClickListener {
            company = Company(null,
                companyNameET.text.toString(),
                descriptionET.text.toString(),
                null,
                null,
                    null,
                streetET.text.toString(),
                cityET.text.toString(),
                zipCodeET.text.toString(),
                null
            )

            val mainIntent = Intent(
                this@RegisterCompanyActivity,
                RegisterCompanyPrimaryUserActivity::class.java
            )

            mainIntent.putExtra("companySerializable", company)
            startActivity(mainIntent)
        }

        signinTxtview.setOnClickListener(View.OnClickListener {
            val mainIntent = Intent(this@RegisterCompanyActivity, LoginActivity::class.java)
            startActivity(mainIntent)
        })
    }
}