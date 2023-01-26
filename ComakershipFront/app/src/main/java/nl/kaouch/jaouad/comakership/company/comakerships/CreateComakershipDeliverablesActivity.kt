package nl.kaouch.jaouad.comakership.company.comakerships

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.widget.Button
import android.widget.ImageView
import androidx.core.view.isVisible
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.textfield.TextInputEditText
import nl.kaouch.jaouad.comakership.models.Deliverable
import nl.kaouch.jaouad.comakership.models.requests.PostCreateComakership
import nl.kaouch.jaouad.comakership.R
import nl.kaouch.jaouad.comakership.RecyclerAdapterDeliverable
import nl.kaouch.jaouad.comakership.company.dashboard.CompanyDashboardActivity
import nl.kaouch.jaouad.comakership.company.inbox.CompanyInboxDashboardActivity
import nl.kaouch.jaouad.comakership.company.profile.CompanyProfileDashboardActivity

class CreateComakershipDeliverablesActivity : AppCompatActivity(){

    private lateinit var comakership: PostCreateComakership
    private lateinit var recyclerview_deliverables: RecyclerView
    private lateinit var recyclerAdapter: RecyclerAdapterDeliverable
    private lateinit var inputFieldDeliverables: TextInputEditText
    private lateinit var deliverables: MutableList<Deliverable>
    private lateinit var nextBtnDeliverables: Button
    private lateinit var addDeliverableBtn: ImageView
    private lateinit var toolbar_back_button: ImageView

    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            checkFieldsForEmptyValues()
        }
    }

    private fun checkFieldsForEmptyValues() {

        val s1: String = inputFieldDeliverables.getText().toString()

        if(s1.isNotEmpty() && s1.length > 8) {
            addDeliverableBtn.isVisible = true
        } else {
            inputFieldDeliverables.setError("It has to include at least 8 characters!")
        }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_create_comakership_deliverables)

        deliverables = mutableListOf()
        inputFieldDeliverables = findViewById(R.id.txt_inputedittxt_deliverables)
        inputFieldDeliverables.setMovementMethod(null)
        recyclerview_deliverables = findViewById(R.id.deliverables_recyclerview)
        nextBtnDeliverables = findViewById(R.id.next_button_deliverables)
        addDeliverableBtn = findViewById(R.id.add_deliverable_btn)
        toolbar_back_button = findViewById(R.id.toolbar_back_button)

        toolbar_back_button.setOnClickListener {
            val intent = Intent(this@CreateComakershipDeliverablesActivity, CompanyComakershipDashboardActivity::class.java)
            this.finish()
            startActivity(intent)
        }

        inputFieldDeliverables.addTextChangedListener(mTextWatcher)

        comakership = (intent.getSerializableExtra("PostCreateComakership") as PostCreateComakership)

        recyclerview_deliverables.setHasFixedSize(true)
        recyclerview_deliverables.layoutManager = LinearLayoutManager(this)


        addDeliverableBtn.setOnClickListener {
            var deliverable = Deliverable(
                null,
                null,
                inputFieldDeliverables.text.toString(),
                null,
                null
            )
            recyclerAdapter = RecyclerAdapterDeliverable(
                baseContext,
                comakership.deliverables.toMutableList()
            )
            if (recyclerAdapter.getList()!!.isNotEmpty()) {
                deliverables = recyclerAdapter.getList() as MutableList<Deliverable>
            }
            deliverables.add(deliverable)
            var freshComakership = PostCreateComakership(
                comakership.name,
                comakership.description,
                emptyList(),
                arrayListOf(),
                comakership.credits,
                comakership.bonus,
                deliverables,
                comakership.purchaseKey
            )
            comakership = freshComakership
            recyclerAdapter = RecyclerAdapterDeliverable(baseContext, deliverables)
            recyclerAdapter.notifyDataSetChanged()
            recyclerview_deliverables.adapter = recyclerAdapter
            inputFieldDeliverables.text = null

            if (!comakership.deliverables.isEmpty()) {
                nextBtnDeliverables.isEnabled = true
            }

            addDeliverableBtn.isVisible = false
        }

        nextBtnDeliverables.setOnClickListener {

            val mainIntent = Intent(
                this@CreateComakershipDeliverablesActivity,
                CreateComakershipSubmitActivity::class.java
            )
            mainIntent.removeExtra("PostCreateComakership")
            mainIntent.putExtra("PostCreateComakership", comakership)
            startActivity(mainIntent)
        }

        var bottomNavigationView: BottomNavigationView = findViewById(R.id.bottom_navigation)
        bottomNavigationView.selectedItemId = R.id.ic_comakerships
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.ic_inbox -> {
                    val intent = Intent(this@CreateComakershipDeliverablesActivity, CompanyInboxDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_home -> {
                    val intent = Intent(this@CreateComakershipDeliverablesActivity, CompanyDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_profile -> {
                    val intent = Intent(this@CreateComakershipDeliverablesActivity, CompanyProfileDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                R.id.ic_comakerships -> {
                    val intent = Intent(this@CreateComakershipDeliverablesActivity, CompanyComakershipDashboardActivity::class.java)
                    this.finish()
                    startActivity(intent)
                    true
                }
                else -> super.onOptionsItemSelected(item)
            }
        }
    }
}