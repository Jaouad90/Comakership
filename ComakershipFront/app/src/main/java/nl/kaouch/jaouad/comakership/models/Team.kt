package nl.kaouch.jaouad.comakership.models

data class Team(
    val id: Int,
    val name: String,
    val linkedStudents: List<LinkedTeam>,
    val joinRequests: String,
    val teamInvites: String,
    val description: String,
)
