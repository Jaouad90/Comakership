package nl.kaouch.jaouad.comakership.models


data class Student (
    var firstName: String,
    var lastName: String,
    val email: String,
    val password: String,
    val programId: Int,
    val nickname: String,
)