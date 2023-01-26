package nl.kaouch.jaouad.comakership.company.dashboard

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ImageView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.MainActivity
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.company.comakerships.CompanyComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity
import nl.kaouch.jaouad.comakership.login.TokenManager

class CompanyDashboardActivity : AppCompatActivity(){

    private lateinit var tokenManager: TokenManager
    private lateinit var logout_button: ImageView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_dashboard_company)

        logout_button = findViewById(R.id.logout_button)
        tokenManager = TokenManager(applicationContext)

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_home
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                val intent = Intent(this@CompanyDashboardActivity, CompanyInboxDashboardActivity::class.java)
                this.finish()
                startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CompanyDashboardActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CompanyDashboardActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CompanyDashboardActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                    this@CompanyDashboardActivity,
                    MainActivity::class.java
            )
            finish()
            startActivity(intent) }
    }
}