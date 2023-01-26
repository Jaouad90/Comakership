package nl.kaouch.jaouad.comakership.models

data class Review(
    val id: Int,
    val companyId: Int,
    val studentUserId: Int,
    val reviewersName: String,
    val rating: Int,
    val comment: String,
    val forCompany: Boolean,
)
