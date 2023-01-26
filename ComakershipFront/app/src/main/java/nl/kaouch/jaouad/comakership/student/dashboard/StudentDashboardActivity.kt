package nl.kaouch.jaouad.comakership.student.dashboard

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ImageView
import com.google.android.material.bottomnavigation.BottomNavigationView
import nl.kaouch.jaouad.comakership.*
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.student.comakership.ComakershipDashboardActivity
import nl.kaouch.jaouad.comakership.student.inbox.InboxActivity
import nl.kaouch.jaouad.comakership.student.profile.StudentProfileActivity
import nl.kaouch.jaouad.comakership.student.team.TeamDashboardActivity

class StudentDashboardActivity : AppCompatActivity() {

    private lateinit var tokenManager: TokenManager
    private lateinit var logout_button: ImageView


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_dashboard_student)

        tokenManager = TokenManager(applicationContext)
        logout_button = findViewById(R.id.logout_button)

        logout_button.setOnClickListener {
            tokenManager.clearJwtToken()
            val intent = Intent(
                this@StudentDashboardActivity,
                MainActivity::class.java
            )
            finish()
            startActivity(intent)
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_home
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_team -> {
                    val intent =
                        Intent(this@StudentDashboardActivity, TeamDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_inbox -> {
                    val intent = Intent(this@StudentDashboardActivity, InboxActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent =
                        Intent(this@StudentDashboardActivity, StudentDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent =
                        Intent(this@StudentDashboardActivity, StudentProfileActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(
                        this@StudentDashboardActivity,
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
}