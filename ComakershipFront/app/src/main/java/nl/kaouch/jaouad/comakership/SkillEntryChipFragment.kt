package com.example.androidmaterialchips

import android.content.Context
import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.view.KeyEvent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import androidx.fragment.app.Fragment
import com.google.android.material.chip.Chip
import com.google.android.material.chip.ChipGroup
import com.google.android.material.textfield.TextInputEditText
import nl.kaouch.jaouad.comakership.models.Skill
import nl.kaouch.jaouad.comakership.R
import java.lang.RuntimeException


class SkillEntryChipFragment : Fragment() {
    private lateinit var skillNames: ArrayList<Skill>
    private lateinit var mView: View
    private lateinit var skillsET: TextInputEditText
    private lateinit var chipGroupSkills: ChipGroup
    private lateinit var addSkillBtn: ImageView


    private val mTextWatcher: TextWatcher = object : TextWatcher {
        override fun beforeTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun onTextChanged(charSequence: CharSequence, i: Int, i2: Int, i3: Int) {}
        override fun afterTextChanged(editable: Editable) {
            skillValidation()
        }
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?,
                              savedInstanceState: Bundle?): View? {
        mView = inflater.inflate(R.layout.fragment_skill_entry_chip, container, false)
        chipGroupSkills = mView.findViewById(R.id.chip_group_skills)
        skillsET = mView.findViewById(R.id.skillEditText)
        addSkillBtn = mView.findViewById(R.id.add_skill_btn)
        skillNames = arrayListOf()
        skillsET.addTextChangedListener(mTextWatcher)
        skillValidation()

        return mView
    }

    private fun skillValidation() {
        var chipExistState = false
        var inputSkill = skillsET.text.toString().replace("\n", "")
        if(inputSkill.isNotEmpty()) {
            addSkillBtn.setOnClickListener {
                chipGroupSkills.checkedChipIds.forEachIndexed { index, _ ->
                    var chip: Chip = chipGroupSkills.getChildAt(index) as Chip
                    if (chip.text.toString() == inputSkill) {
                        chipExistState = true
                        skillsET.error =
                            "This skill already exists!"
                    }
                }
                if (!chipExistState) {
                    chipExistState = false
                    addChipToGroup(inputSkill, chipGroupSkills)
                    pListener.setSkills(skillNames)
                    skillsET.text = null
                    inputSkill = ""
                }
            }
        } else{
            skillsET.error =
                "Add max 5 skills to the comakership"
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)

        if(context is ISkillListener) {
            pListener = context
        } else {
            throw RuntimeException(context.toString() + "Missing a ISkillListener")
        }
    }

    private fun addChipToGroup(skill: String, chipGroup: ChipGroup) {
        if(chipGroup.checkedChipIds.size < 5) {
            val chip = Chip(context)
            chip.text = skill
            chip.isCloseIconVisible = true
            chip.setCloseIconTintResource(R.color.primary_green)
            // necessary to get single selection working
            skillNames.add(Skill(null, skill))
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
        fun skillFragment() = SkillEntryChipFragment()
    }

    lateinit var pListener: ISkillListener

    interface ISkillListener {
        fun setSkills(chosenSkills: ArrayList<Skill>)
    }
}