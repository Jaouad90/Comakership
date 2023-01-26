package nl.kaouch.jaouad.comakership.models.responses

import nl.kaouch.jaouad.comakership.models.TeamApplication

data class ComakershipApplications(
    val studentUserId: Int,
    val teamId: Int,
    val team: TeamApplication,

    )
