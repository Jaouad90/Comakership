package nl.kaouch.jaouad.comakership.models.requests

import nl.kaouch.jaouad.comakership.models.*
import java.io.Serializable

data class PostCreateComakership(
    val name: String,
    val description: String,
    val skills: List<Skill>,
    val programIds: ArrayList<Int>,
    val credits: Boolean,
    val bonus: Boolean,
    val deliverables: List<Deliverable>,
    val purchaseKey: String
) : Serializable