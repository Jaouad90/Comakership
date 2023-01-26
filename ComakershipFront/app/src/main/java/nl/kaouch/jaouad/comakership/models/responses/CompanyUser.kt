package nl.kaouch.jaouad.comakership.models.responses

import nl.kaouch.jaouad.comakership.models.Company
import java.io.Serializable

data class CompanyUser(
    var id: Int?,
    var name: String,
    var email: String?,
    var password: String?,
    var company: Company?,
    var isCompanyAdmin: Boolean?,
    var deleted: Boolean?
) : Serializable