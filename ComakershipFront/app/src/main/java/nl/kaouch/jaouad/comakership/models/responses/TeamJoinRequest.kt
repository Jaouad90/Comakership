package nl.kaouch.jaouad.comakership.models.responses

import nl.kaouch.jaouad.comakership.models.Team

data class TeamJoinRequest(
    var studentUserId: Int,
    var teamId: Int,
    var studentUser: StudentUser,
    var team: Team,
)