package nl.kaouch.jaouad.comakership.models.requests

data class PostAddCompanyUser(
    val userEmail: String,
    val makeAdmin: Boolean,
)