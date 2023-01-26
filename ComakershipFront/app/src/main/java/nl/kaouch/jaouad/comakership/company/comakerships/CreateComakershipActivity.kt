package nl.kaouch.jaouad.comakership.company.comakerships

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.widget.*
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.models.requests.PostCreateComakership
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity

class CreateComakershipActivity : AppCompatActivity() {

    private lateinit var next_btn: Button
    private lateinit var projectname_et : EditText
    private lateinit var projectdescription_et : EditText
    private lateinit var purchasekey_et : EditText
    private lateinit var bonus_checkbox : CheckBox
    private lateinit var credits_checkbox : CheckBox
    private lateinit var toolbar_back_button: ImageView

    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            checkFieldsForEmptyValues()
        }
    }

    private fun checkFieldsForEmptyValues() {

        val s1: String = projectname_et.getText().toString()
        val s2: String = projectdescription_et.getText().toString()
        val s3: String = purchasekey_et.getText().toString()

        next_btn.isEnabled = !(s1 == "" || s2 == "" || s3 == "")
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_create_comakership)

        projectname_et = findViewById(R.id.projectname_et)
        projectdescription_et = findViewById(R.id.projectdescription_et)
        purchasekey_et = findViewById(R.id.purchasekey_et)
        bonus_checkbox = findViewById(R.id.bonus_checkbox)
        credits_checkbox = findViewById(R.id.credits_checkbox)
        next_btn = findViewById(R.id.next_btn)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@CreateComakershipActivity, CompanyComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        projectname_et.addTextChangedListener(mTextWatcher)
        projectdescription_et.addTextChangedListener(mTextWatcher)
        purchasekey_et.addTextChangedListener(mTextWatcher)

        next_btn.setOnClickListener {
            var comakership = PostCreateComakership(
                projectname_et.text.toString(),
                projectdescription_et.text.toString(),
                emptyList(),
                arrayListOf(),
                credits_checkbox.isChecked,
                bonus_checkbox.isChecked,
                arrayListOf(),
                purchasekey_et.text.toString()
            )
            val mainIntent = Intent(
                this@CreateComakershipActivity,
                CreateComakershipDeliverablesActivity::class.java
            )
            mainIntent.putExtra("PostCreateComakership", comakership)
            startActivity(mainIntent)
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CreateComakershipActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CreateComakershipActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CreateComakershipActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CreateComakershipActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }
}