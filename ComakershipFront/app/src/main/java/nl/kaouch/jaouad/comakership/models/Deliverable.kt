package nl.kaouch.jaouad.comakership.models

import java.io.Serializable

data class Deliverable(
    val id: Int?,
    val comakershipId: Int?,
    val name: String,
    val finished: Boolean?,
    val comakership: Comakership?
): Serializable
