package nl.kaouch.jaouad.comakership.models

import nl.kaouch.jaouad.comakership.models.responses.CompanyUser
import java.io.Serializable

data class Company(
    val id: Int?,
    val name: String,
    val description: String,
    val registrationDate: String?,
    val reviews: List<Review>?,
    val comakerships: List<Comakership>?,
    val street: String,
    val city: String,
    val zipcode: String,
    var CompanyUser: CompanyUser?,
) : Serializable