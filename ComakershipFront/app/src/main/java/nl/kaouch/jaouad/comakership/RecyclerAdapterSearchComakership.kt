package nl.kaouch.jaouad.comakership

import android.content.Context
import android.content.Intent
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.core.view.isVisible
import androidx.recyclerview.widget.RecyclerView
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.login.LoginActivity
import nl.kaouch.jaouad.comakership.login.TokenManager
import nl.kaouch.jaouad.comakership.models.Comakership
import nl.kaouch.jaouad.comakership.student.comakership.SearchComakershipAlertDialogActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory


class RecyclerAdapterSearchComakership(
    private val context: Context,
    private val comakerships: List<Comakership>?,
    private val listener: onComakershipClickListener
) : RecyclerView.Adapter<RecyclerAdapterSearchComakership.ViewHolder>() {

    private lateinit var tokenManager: TokenManager
    private lateinit var alreadyJoinedComakerships: List<Comakership>

    override fun onCreateViewHolder(
        parent: ViewGroup,
        viewType: Int
    ): RecyclerAdapterSearchComakership.ViewHolder {
        val comakershipView = LayoutInflater.from(context).inflate(
            R.layout.card_layout_student_search_comakership,
            parent,
            false
        )
        alreadyJoinedComakerships = emptyList()
        tokenManager = TokenManager(context)
        return ViewHolder(comakershipView)
    }

    override fun onBindViewHolder(
        holder: RecyclerAdapterSearchComakership.ViewHolder,
        position: Int
    ) {
        val call1 = fetchApi().getComakerShipsOfUser("Bearer " + tokenManager.getToken())
        call1.enqueue(object : Callback<List<Comakership>> {

            override fun onResponse(
                call: Call<List<Comakership>>,
                response: Response<List<Comakership>>
            ) {
                if (response.code() == 401) {
                    tokenManager.clearJwtToken()
                    val intent = Intent(context, LoginActivity::class.java)
                    context.startActivity(intent)
                }
                if (response.isSuccessful) {
                    var responseBody = response.body()!!
                    if (!responseBody.isNullOrEmpty()) {
                        alreadyJoinedComakerships += responseBody
                        alreadyJoinedComakerships.forEach {
                            comakerships!!.toMutableList().removeAll { item ->
                                item.id == it.id
                            }
                        }
                    }
                        if (!comakerships.isNullOrEmpty()) {
                        comakerships!!.forEach {

                                holder.itemName.text = comakerships[position].name
                                holder.itemStatus.text = ""
                                holder.itemBtn.isVisible = true
                                holder.itemBtn.isEnabled = true
                                holder.itemBtn.setOnClickListener {
                                    val intent = Intent(
                                        context,
                                        SearchComakershipAlertDialogActivity::class.java
                                    )
                                    intent.putExtra("chosenComakershipId", comakerships[position].id!!)
                                    context.startActivity(intent)
                                }
                        }

                    } else {
                        holder.itemName.text = "No existing comakerships found!!"
                    }

                }
            }

            override fun onFailure(call: Call<List<Comakership>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
            }
        })
    }

    override fun getItemCount(): Int {
        if (comakerships != null) {
            if (!comakerships.isNullOrEmpty()) {
                return comakerships.size
            }
        }
        return -1
    }

    inner class ViewHolder(articleView: View) : RecyclerView.ViewHolder(articleView),
        View.OnClickListener {

        var itemName: TextView = itemView.findViewById(R.id.comakership_name)
        var itemStatus: TextView = itemView.findViewById(R.id.comakership_status)
        var itemBtn: TextView = itemView.findViewById(R.id.apply_comakership_btn)

        init {
            itemView.setOnClickListener(this)
        }

        override fun onClick(v: View?) {
            val position: Int = adapterPosition
            if (position != RecyclerView.NO_POSITION)
                listener.onComakershipClick(position)
        }
    }

    interface onComakershipClickListener {
        fun onComakershipClick(position: Int)
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