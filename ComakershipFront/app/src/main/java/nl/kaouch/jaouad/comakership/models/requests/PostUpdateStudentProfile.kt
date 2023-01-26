package nl.kaouch.jaouad.comakership.models.requests

data class PostUpdateStudentProfile(
    val name: String,
    val about: String,
    val links: List<String>,
    val nickname: String,
)