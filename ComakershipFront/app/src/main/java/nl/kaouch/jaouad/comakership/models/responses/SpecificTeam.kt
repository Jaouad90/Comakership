package nl.kaouch.jaouad.comakership.models.responses

import nl.kaouch.jaouad.comakership.models.TeamMember

data class SpecificTeam(
    val id: Int,
    val name: String,
    val description: String,
    val members: List<TeamMember>,
)