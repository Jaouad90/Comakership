package nl.kaouch.jaouad.comakership.models.requests

data class PostChangePassword(
    val oldPassword: String,
    val newPassword: String,
    val confirmNewPassword: String,
)
