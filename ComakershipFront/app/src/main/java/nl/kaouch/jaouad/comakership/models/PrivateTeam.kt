package nl.kaouch.jaouad.comakership.models

data class PrivateTeam(
    val id: Int,
    val name: String,
    val description: String,
    val linkedStudents: List<LinkedStudent>,
)
