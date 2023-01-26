package nl.kaouch.jaouad.comakership.models

import java.io.Serializable

data class Comakership(
    val id: Int?,
    val name: String,
    val description: String,
    val company: Company?,
    val status: Status?,
    val skills: List<Skill>,
    val programs: List<Program>,
    val credits: Boolean,
    val bonus: Boolean,
    val companyId: Int?,
    val createdAt: String,
    val students: List<Student>,
    val deliverables: List<Deliverable>,
) : Serializable
