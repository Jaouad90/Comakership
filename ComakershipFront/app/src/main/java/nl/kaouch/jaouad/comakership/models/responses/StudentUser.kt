package nl.kaouch.jaouad.comakership.models.responses

import nl.kaouch.jaouad.comakership.models.JoinRequest
import nl.kaouch.jaouad.comakership.models.LinkedTeam
import nl.kaouch.jaouad.comakership.models.PrivateTeam


data class StudentUser(
        val studentNumber: Int,
        val about: String,
        val nickname: String,
        val privateTeamId: Int,
        val links: List<String>,
        val id: Int,
        val name: String,
        val email: String,
        val password: String,
        val deleted: String,
        val privateTeam: PrivateTeam,
        val linkedTeams: List<LinkedTeam>,
        val joinRequests: List<JoinRequest>,
        )



