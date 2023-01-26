using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Dahomey.Json;
using Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ServiceLayer;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using ServiceLayer.User;

namespace ComakershipsApi
{
    class TeamController{

        private readonly ILogger _logger;
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        private readonly JsonSerializerOptions _options;

        public TeamController(ILogger<TeamController> logger, ITeamService teamService, IUserService userService)
        {
            _logger = logger;
            _teamService = teamService;
            _userService = userService;
            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }.SetupExtensions();
        }

        /// <summary>
        /// The method allows you to retreive all teams
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetTeams")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeams([HttpTrigger(AuthorizationLevel.User, "get", Route = "teams")] HttpRequest req) {
            _logger.LogInformation("Getting all teams.");

            var teams = await _teamService.GetTeams();
            return new OkObjectResult(teams);
        }

        /// <summary>
        /// This method allows you to retreive a specific team, by supplying the get method with an id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("GetTeam")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeam([HttpTrigger(AuthorizationLevel.User, "get", Route = "teams/{id:int}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Getting team: " + id);

            var team = await _teamService.GetTeam(id);
            if (team != null)
            {
                return new OkObjectResult(team);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// This method allows you to retreive a specific team, by supplying the get method with an id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("GetTeamComplete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamComplete([HttpTrigger(AuthorizationLevel.User, "get", Route = "teams/complete/{id:int}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Getting team(complete): " + id);

            var team = await _teamService.GetTeamComplete(id);
            if (team != null)
            {
                return new OkObjectResult(team);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// The method allows you to form a new team by doing a post request
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("CreateTeam")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTeam(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "teams")]
            [RequestBodyType(typeof(TeamPost), "The team to store")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            _logger.LogInformation("Adding team.");

            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.FindFirst("UserId");
            if (userId != null)
            {
                try
                {
                    var postData = await JsonSerializer.DeserializeAsync<TeamPost>(req.Body, _options);
                    try
                    {
                        int newTeamId = await _teamService.CreateTeam(postData, int.Parse(userId.Value));
                        return new OkObjectResult($"Team with id '{newTeamId}' added");
                    }
                    catch (Exception e)
                    {
                        return new UnprocessableEntityObjectResult(e.Message);
                    }                    
                }
                catch (JsonException e)
                {
                    return new BadRequestObjectResult(e.Message);
                }
            }
            else return new BadRequestObjectResult("Failed to verify logged-in user, please try logging in again");
        }

        /// <summary>
        /// This method allows you to edit a Team entity
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("EditTeam")]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditTeam([HttpTrigger(AuthorizationLevel.User, "put", Route = "teams/{id:int}")]
                                                   [RequestBodyType(typeof(TeamPut), "The team to edit")] HttpRequest req, int id)
        {
            _logger.LogInformation("Updating team.");

            try
            {
                var putData = await JsonSerializer.DeserializeAsync<TeamPut>(req.Body, _options);

                if (putData.Id == id)
                {
                    if (await _teamService.UpdateTeam(putData))
                    {
                        return new AcceptedResult();
                    }
                    return new UnprocessableEntityObjectResult("Failed to update");
                }
                else return new BadRequestObjectResult("Id's of data and endpoint dont match");
            }
            catch (JsonException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }       

        /// <summary>
        /// allows a team to apply for a comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="teamid">Integer</param>
        /// <param name="comakershipid">Integer</param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("ApplyForComakership")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApplyForComakership(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "teams/{teamid:int}/applyforcomakership/{comakershipid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User,
            int teamid, 
            int comakershipid) {

            _logger.LogInformation("Applying for comakership.");

            var identity = (ClaimsIdentity)User.Identity;

            try {
                if(await _teamService.ApplyForComakership(teamid, comakershipid, identity)) {
                    return new OkObjectResult("Successfully applied for comakership");
                } else {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }
            } catch(UnauthorizedAccessException e) {
                return new UnauthorizedObjectResult(e.Message);
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a team to cancel their application for a comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="teamid">Integer</param>
        /// <param name="comakershipid">Integer</param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("CancelApplyForComakership")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelApplyForComakership(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = "teams/{teamid:int}/applyforcomakership/{comakershipid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User,
            int teamid,
            int comakershipid)
        {
            _logger.LogInformation("Cancelling application for comakership.");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.CancelApplyForComakership(teamid, comakershipid, identity))
                {
                    return new OkObjectResult("Cancelled application for comakership");
                }
                else
                {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a user to request to join the team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("JoinTeamRequest")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> JoinTeamRequest(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "teams/{teamid:int}/join")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int teamid)
        {
            _logger.LogInformation("Requesting to join team.");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.CreateJoinRequest(teamid, identity))
                {
                    return new OkObjectResult("Requested to join team");
                }
                else
                {
                    return new ObjectResult("Something went wrong, try again later") { StatusCode = 500 };
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a user to cancel their request to join the team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("CancelJoinTeamRequest")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelJoinTeamRequest(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = "teams/{teamid:int}/join")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int teamid)
        {
            _logger.LogInformation("Cancelling join request.");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.CancelJoinRequest(teamid, identity))
                {
                    return new OkObjectResult("Cancelled request to join team");
                }
                else
                {
                    return new ObjectResult("Something went wrong, try again later") { StatusCode = 500 };
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Retrieves all of a teams joinrequests from students
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("GetJoinRequests")]
        [ProducesResponseType(typeof(List<JoinRequestGet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJoinRequests([HttpTrigger(AuthorizationLevel.User, "get", Route = "teams/{teamid:int}/joinrequests")] HttpRequest req, int teamid,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;

            var joinrequests = await _teamService.GetJoinRequests(teamid, identity);
            return new OkObjectResult(joinrequests);
        }

        /// <summary>
        /// Retrieves all joinrequests for a student
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        [FunctionName("GetSentJoinRequests")]
        [ProducesResponseType(typeof(List<JoinRequestGet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentJoinRequests([HttpTrigger(AuthorizationLevel.User, "get", Route = "student/joinrequests")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;

            var joinrequests = await _teamService.GetSentJoinRequests(identity);
            return new OkObjectResult(joinrequests);
        }

        /// <summary>
        /// allows a team to accept a joinrequest from a student
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <param name="studentid">Integer</param>
        /// <returns></returns>
        [FunctionName("AcceptJoinRequest")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptJoinRequest([HttpTrigger(AuthorizationLevel.User, "post", Route = "teams/{teamid:int}/joinrequests/{studentid:int}")] HttpRequest req, int teamid, int studentid,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.AcceptApplication(teamid, studentid, identity))
                {
                    return new OkObjectResult("Sucessfully accepted application");
                }
                else
                {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a team to reject a joinrequest from a student
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <param name="studentid">Integer</param>
        /// <returns></returns>
        [FunctionName("RejectJoinRequest")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectJoinRequest([HttpTrigger(AuthorizationLevel.User, "delete", Route = "teams/{teamid:int}/joinrequests/{studentid:int}")] HttpRequest req, int teamid, int studentid,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.RejectApplication(teamid, studentid, identity))
                {
                    return new OkObjectResult("Sucessfully rejected application");
                }
                else
                {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a team to request a student to join the team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <param name="email"></param>
        /// <returns></returns>
        [FunctionName("TeamInviteRequest")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TeamInviteRequest(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "teams/{teamid:int}/invite/{email}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int teamid, string email)
        {
            _logger.LogInformation("Inviting student.");

            var identity = (ClaimsIdentity)User.Identity;
            var user = (StudentUser)await _userService.GetSingle(u => u.Email == email);

            if (user != null)
            {
                try
                {
                    if (await _teamService.CreateTeamInvite(teamid, user.Id, identity))
                    {
                        return new OkObjectResult("Invite sent");
                    }
                    else
                    {
                        return new ObjectResult("Something went wrong, try again later") { StatusCode = 500 };
                    }
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e.Message);
                }
            }
            return new NotFoundObjectResult("No user found with that email");                        
        }

        /// <summary>
        /// allows a team to cancel their invite to join the team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <param name="studentid">Integer</param>
        /// <returns></returns>
        [FunctionName("CancelTeamInviteRequest")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelTeamInviteRequest(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = "teaminvites/{teamid:int}/{studentid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int teamid, int studentid)
        {
            _logger.LogInformation("Cancelling invite.");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.CancelTeamInviteRequest(teamid, studentid, identity))
                {
                    return new OkObjectResult("Cancelled request to join team");
                }
                else
                {
                    return new ObjectResult("Something went wrong, try again later") { StatusCode = 500 };
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a student to get all received teaminvites
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("GetTeamInvites")]
        [ProducesResponseType(typeof(List<TeamInviteGet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamInvites([HttpTrigger(AuthorizationLevel.User, "get", Route = "student/teaminvites")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;

            var teaminvites = await _teamService.GetTeamInvites(identity);
            return new OkObjectResult(teaminvites);
        }

        /// <summary>
        /// allows a team to get all sent teaminvites
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("GetSentTeamInvites")]
        [ProducesResponseType(typeof(List<TeamInviteGet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSentTeamInvites([HttpTrigger(AuthorizationLevel.User, "get", Route = "team/{teamid:int}/teaminvites")] HttpRequest req, int teamid,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;

            var teaminvites = await _teamService.GetSentTeamInvites(teamid, identity);
            return new OkObjectResult(teaminvites);         
        }

        /// <summary>
        /// allows a student to accept an invite from a team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("AcceptTeamInvite")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AcceptTeamInvite([HttpTrigger(AuthorizationLevel.User, "post", Route = "teaminvites/{teamid:int}")] HttpRequest req, int teamid,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.AcceptTeamInvite(teamid, identity))
                {
                    return new OkObjectResult("Sucessfully accepted invite");
                }
                else
                {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a student to reject an invite from a team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("RejectTeamInvite")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectTeamInvite([HttpTrigger(AuthorizationLevel.User, "delete", Route = "teaminvites/{teamid:int}")] HttpRequest req, int teamid,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.RejectTeamInvite(teamid, identity))
                {
                    return new OkObjectResult("Sucessfully rejected invite");
                }
                else
                {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }
            }
            catch (UnauthorizedAccessException e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a team to get all comakership applications they made
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("GetTeamApplications")]
        [ProducesResponseType(typeof(List<TeamComakershipComakershipGet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamApplications([HttpTrigger(AuthorizationLevel.User, "get", Route = "teams/{id:int}/applications")] HttpRequest req, int id,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;

            var applications = await _teamService.GetApplications(id, identity);
            return new OkObjectResult(applications);
        }

        /// <summary>
        /// allows a user to leave a team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("LeaveTeam")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LeaveTeam(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "teams/{teamid:int}/leave")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int teamid)
        {
            _logger.LogInformation("Leaving team.");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.LeaveTeam(teamid, identity))
                {
                    return new OkObjectResult("Left the team");
                }
                else
                {
                    return new ObjectResult("Something went wrong, try again later") { StatusCode = 500 };
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// allows a team member to kick another member
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="teamid">Integer</param>
        /// <param name="studentid">Integer</param>
        /// <returns></returns>
        [FunctionName("KickTeamMember")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> KickTeamMember(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "teams/{teamid:int}/kick/{studentid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int teamid, int studentid)
        {
            _logger.LogInformation("Kicking student");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _teamService.KickMember(teamid, studentid, identity))
                {
                    return new OkObjectResult("Removed from the team");
                }
                else
                {
                    return new ObjectResult("Something went wrong, try again later") { StatusCode = 500 };
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
