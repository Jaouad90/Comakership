package nl.kaouch.jaouad.comakership.models

data class TeamApplication(
    val id: Int,
    val name: String,
    val description: String,
    val members: List<Member>,
)