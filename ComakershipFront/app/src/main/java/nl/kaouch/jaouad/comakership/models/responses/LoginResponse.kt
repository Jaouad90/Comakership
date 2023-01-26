package nl.kaouch.jaouad.comakership.models.responses

data class LoginResponse(
    val UserType: String,
    val UserId: String,
    val Token: String,
    val companyId: Int,
)
