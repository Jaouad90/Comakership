package nl.kaouch.jaouad.comakership.API

import nl.kaouch.jaouad.comakership.models.*
import nl.kaouch.jaouad.comakership.models.requests.*
import nl.kaouch.jaouad.comakership.models.responses.*
import okhttp3.MultipartBody
import retrofit2.Call
import retrofit2.http.*

interface ApiInterface {

    @POST("review")
    fun reviewCompany(@Body review: PostReview, @Header("Authorization") token: String): Call<Void>

    @GET("deliverables/{id}")
    fun getDeliverable(@Header("Authorization") token: String, @Path("id") id: Int): Call<Deliverable>

    @Multipart
    @POST("comakerships/{id}/file")
    fun uploadDeliverable(@Header("Authorization") token: String, @Path("id") id: Int, @Part file: MultipartBody.Part): Call<Void>

    @POST("comakerships/{id}/file")
    fun downloadDeliverable(@Header("Authorization") token: String, @Path("id") id: Int): Call<LoginResponse>

    @PUT("deliverables/{id}")
    fun updateDeliverable (@Header("Authorization") token: String, @Path("id") id: Int, @Body deliverable: Deliverable): Call<Void>

    @PUT("comakerships/{id}")
    fun updateComakershipStatus (@Header("Authorization") token: String, @Path("id") id: Int, @Body comakership: PutComakership): Call<Void>

    @POST("comakerships/{comakershipid}/acceptapplication/{teamid}")
    fun acceptApplicationOfTeam(@Header("Authorization") token: String, @Path("teamid") teamid: Int, @Path("comakershipid") comakershipid: Int): Call<Void>

    @POST("comakerships/{comakershipid}/rejectapplication/{teamid}")
    fun rejectApplicationOfTeam(@Header("Authorization") token: String, @Path("teamid") teamid: Int, @Path("comakershipid") comakershipid: Int): Call<Void>

    @GET("comakerships/search")
    fun searchComakerships(@Query("Skill") Skill: String): Call<List<Comakership>>

    @GET("comakerships/{comakershipId}/deliverables")
    fun getComakershipDeliverables(@Header("Authorization") token: String, @Path("comakershipId") id: Int): Call<List<Deliverable>>

    @GET("comakerships/{id}/applications")
    fun getApplicationsOfComakership(@Header("Authorization") token: String, @Path("id") id: Int): Call<List<ComakershipApplications>>

    @GET("comakerships/{id}")
    fun getComakership(@Header("Authorization") token: String, @Path("id") id: Int): Call<Comakership>

    @POST("teams/{teamid}/applyforcomakership/{comakershipid}")
    fun applyComakershipAsTeam(@Header("Authorization") token: String, @Path("teamid") teamid: Int, @Path("comakershipid") comakershipid: Int): Call<Void>

    @POST("comakerships")
    fun createComakership(@Body comakership: PostCreateComakership, @Header("Authorization") token: String): Call<Void>

    @GET("comakerships/loggedinuser/all")
    fun getComakerShipsOfUser(@Header("Authorization") token: String): Call<List<Comakership>>

    @GET("teams/{id}/applications")
    fun getApplicationsOfTeam(@Header("Authorization") token: String, @Path("id") id: Int): Call<List<Comakership>>

    @GET("programs")
    fun getPrograms(): Call<ArrayList<Program>>

    @GET("universities/domains")
    fun getDomains(): Call<List<UniversityDomain>>

    @PUT("company/{id}")
    fun setCompanyInfo(@Header("Authorization") token: String, @Path("id") id: Int, @Body company: Company): Call<Void>

    @GET("company/{id}")
    fun getCompany(@Header("Authorization") token: String, @Path("id") id: Int): Call<Company>

    @GET("CompanyUser/{id}")
    fun getCompanyUser(@Header("Authorization") token: String, @Path("id") id: Int): Call<CompanyUser>

    @PUT("Students")
    fun setStudentInfo(@Header("Authorization") token: String, @Body student: PostUpdateStudentProfile): Call<Void>

    @GET("Students/{id}")
    fun getStudentUser(@Header("Authorization") token: String, @Path("id") id: Int): Call<StudentUser>

    @GET("teams/complete/{id}")
    fun getTeam(@Header("Authorization") token: String, @Path("id") id: Int): Call<SpecificTeam>

    @GET("teams")
    fun getTeams(@Header("Authorization") token: String): Call<List<PrivateTeam>>

    @POST("teams/{teamid}/join")
    fun joinTeam(@Header("Authorization") token: String, @Path("teamid") teamid: Int): Call<Void>

    @POST("teams/{teamid}/leave")
    fun leaveTeam(@Header("Authorization") token: String, @Path("teamid") teamid: Int): Call<Void>

    @POST("teams/{teamid}/joinrequests/{studentid}")
    fun acceptJoinRequest(@Header("Authorization") token: String, @Path("teamid") teamid: Int, @Path("studentid") studentid: Int): Call<Void>

    @DELETE("teams/{teamid}/joinrequests/{studentid}")
    fun rejectJoinRequest(@Header("Authorization") token: String, @Path("teamid") teamid: Int, @Path("studentid") studentid: Int): Call<Void>

    @GET("teams/{teamid}/joinrequests")
    fun getTeamJoinRequests(@Header("Authorization") token: String, @Path("teamid") teamid: Int): Call<List<TeamJoinRequest>>

    @POST("teams")
    fun addTeam(@Header("Authorization") token: String, @Body postTeam: PostTeam): Call<Void>

    @PUT("CompanyUser")
    fun setCompanyUserInfo(@Header("Authorization") token: String, @Body name: CompanyUser): Call<Void>

    @POST("company/{id}/addcompanyuser")
    fun addCompanyUser(@Header("Authorization") token: String, @Body postAddCompanyUser: PostAddCompanyUser, @Path("id") id: Int): Call<Void>

    @POST("User/ChangePassword")
    fun setUserPassword(@Header("Authorization") token: String, @Body postChangePassword: PostChangePassword): Call<Void>

    @POST("Login")
    fun login(@Body credentials: LoginRequest): Call<LoginResponse>

    @POST("company")
    fun registerCompany(@Body company: Company): Call<Void>

    @POST("Students")
    fun registerStudent(@Body student: Student): Call<Void>

}