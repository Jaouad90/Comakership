package nl.kaouch.jaouad.comakership.models.requests

data class PutComakership(
    var id: Int,
    var name: String,
    var description: String,
    var credits: Boolean,
    var bonus: Boolean,
    var comakershipStatusId: Int,
)