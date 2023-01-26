package nl.kaouch.jaouad.comakership.models.requests

data class PostReview(
    val companyId: Int,
    val studentUserId: Int,
    val rating: Int,
    val comment: String,
    val forCompany: Boolean,
)
