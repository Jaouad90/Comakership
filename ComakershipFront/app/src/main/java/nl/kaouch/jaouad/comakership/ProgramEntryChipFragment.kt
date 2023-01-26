package com.example.androidmaterialchips

import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import android.widget.AutoCompleteTextView
import android.widget.Toast
import androidx.fragment.app.Fragment
import com.google.android.material.chip.Chip
import com.google.android.material.chip.ChipGroup
import nl.kaouch.jaouad.comakership.API.ApiInterface
import nl.kaouch.jaouad.comakership.BASE_URL
import nl.kaouch.jaouad.comakership.models.Program
import nl.kaouch.jaouad.comakership.R
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.lang.RuntimeException


class ProgramEntryChipFragment : Fragment() {
    private lateinit var mView: View
    private lateinit var programIds: ArrayList<Int>
    private lateinit var programNames: ArrayList<String>
    private lateinit var chosenProgramIds: ArrayList<Int>
    private lateinit var autoCompleteTxtV: AutoCompleteTextView
    private lateinit var chipGroupPrograms: ChipGroup

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        mView = inflater.inflate(R.layout.fragment_program_entry_chip, container, false)
        chipGroupPrograms = mView.findViewById(R.id.chip_group_programs)

        programIds = arrayListOf()
        programNames = arrayListOf()
        chosenProgramIds = arrayListOf()
        getPrograms()

        return mView
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)

        if(context is IProgramListener) {
            pListener = context
        } else {
            throw RuntimeException(context.toString() + "Missing a IProgramListener")
        }
    }

    private fun addChipToGroup(program: String, chipGroup: ChipGroup) {
        if(chipGroup.checkedChipIds.size < 5) {
            val chip = Chip(context)
            chip.text = program
            chip.isCloseIconVisible = true
            chip.setCloseIconTintResource(R.color.primary_green)
            chipGroup.checkedChipIds
            // necessary to get single selection working
            chip.isClickable = true
            chip.isCheckable = true
            chip.isChecked = true
            chip.isCheckedIconVisible = false
            chipGroup.addView(chip as View)
            chip.setOnCloseIconClickListener { chipGroup.removeView(chip as View) }
        }
    }

    companion object {
        @JvmStatic
        fun newInstance() = ProgramEntryChipFragment()
    }

    private fun getPrograms() {
        val call = fetchApi().getPrograms()
        call.enqueue(object : Callback<ArrayList<Program>> {

            override fun onResponse(
                call: Call<ArrayList<Program>>,
                response: Response<ArrayList<Program>>
            ) {
                if (response.isSuccessful) {
                    var responseBody = response.body()
                    for (program in responseBody!!) {
                        programNames.add(program.name!!)
                        programIds.add(program.id)
                    }

                    val adapter = ArrayAdapter<String>(
                        requireActivity(),
                        android.R.layout.simple_dropdown_item_1line, programNames
                    )
                    autoCompleteTxtV = mView.findViewById(R.id.autoCompleteTextView)
                    autoCompleteTxtV.setAdapter(adapter)
                    autoCompleteTxtV.setOnItemClickListener { parent, _, position, _ ->
                        autoCompleteTxtV.text = null
                        val selected = parent.getItemAtPosition(position) as String
                        for (program in responseBody) {
                            if (program.name == selected) {
                                chosenProgramIds.add(program.id)
                            }
                        }
                        addChipToGroup(selected, chipGroupPrograms)
                    }
                    pListener.setPrograms(chosenProgramIds)
                } else {
                    Toast.makeText(
                        mView.context,
                        "There went something wrong!!",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }

            override fun onFailure(call: Call<ArrayList<Program>>, t: Throwable) {
                Log.e("HTTP", "Could not fetch data", t)
                Toast.makeText(
                    mView.context,
                    "Check the internet connection!",
                    Toast.LENGTH_SHORT
                ).show()

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

    lateinit var pListener: IProgramListener

    interface IProgramListener {
        fun setPrograms(chosenPrograms: ArrayList<Int>)
    }
}