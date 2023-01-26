package nl.kaouch.jaouad.comakership

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.TextView
import androidx.constraintlayout.widget.Group
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.register.company.RegisterCompanyActivity
import nl.kaouch.jaouad.comakership.register.student.RegisterStudentActivity

//const val BASE_URL = "http://192.168.1.159:7071/api/"
const val BASE_URL = "http://10.0.2.2:7071/api/"

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        home()
    }

    private fun home() {
        var companyGroup: Group = findViewById(R.id.companyGroup)
        var studentGroup: Group = findViewById(R.id.studentGroup)
        var signinTxtview: TextView = findViewById(R.id.signinAnswerTXT)

        companyGroup.setAllOnClickListener(View.OnClickListener {
            val mainIntent = Intent(this@MainActivity, RegisterCompanyActivity::class.java)
            startActivity(mainIntent)
        })

        studentGroup.setAllOnClickListener(View.OnClickListener {
            val mainIntent = Intent(this@MainActivity, RegisterStudentActivity::class.java)
            startActivity(mainIntent)
        })

        signinTxtview.setOnClickListener(View.OnClickListener {
            val mainIntent = Intent(this@MainActivity, LoginActivity::class.java)
            startActivity(mainIntent)
        })
    }

    fun Group.setAllOnClickListener(listener: View.OnClickListener?) {
        referencedIds.forEach { id ->
            rootView.findViewById<View>(id).setOnClickListener(listener)
        }
    }
}