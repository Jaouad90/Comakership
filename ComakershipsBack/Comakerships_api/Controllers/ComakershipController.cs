using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dahomey.Json;
using System.Text.Json;
using Models;
using ServiceLayer;
using System.Security.Claims;
using System.Linq;

namespace ComakershipsApi.Controllers
{
    /// <summary>
    /// class definition for the ComakershipController. It contains endpoints related to Comakerhips.
    /// </summary>
    public class ComakershipController
    {
        private readonly ILogger _logger;
        private readonly IComakershipService _comakershipService;
        JsonSerializerOptions _options;

        /// <summary>
        /// Constructor for the ComakershipController
        /// </summary>
        public ComakershipController(ILogger<ComakershipController> logger, IComakershipService comakershipService)
        {
            _logger = logger;
            _comakershipService = comakershipService;
            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true                
            }.SetupExtensions();
        }

        /// <summary>
        /// Finds and returns all Comakerships
        /// </summary>
        /// <returns></returns>
        [FunctionName("GetAllComakerships")]
        [ProducesResponseType(typeof(List<ComakershipBasic>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllComakerships([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "comakerships")] HttpRequest req)
        {
            _logger.LogInformation("Getting all comakerships.");

            var comakerships = await _comakershipService.GetComakerships();
            return new OkObjectResult(comakerships);
        }

        /// <summary>
        /// Finds a Comakership by id
        /// </summary>
        /// <param name="id">Integer</param>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetComakership")]
        [ProducesResponseType(typeof(ComakershipBasic), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComakership([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "comakerships/{id:int}")] HttpRequest req, int id) {
            _logger.LogInformation("Getting comakership: " + id);

            var comakership = await _comakershipService.GetComakership(id);
            if (comakership != null) {
                return new OkObjectResult(comakership);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds a Comakership by id, includes related Deliverables and Skills
        /// </summary>
        /// <param name="id">Integer</param>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetComakershipComplete")]
        [ProducesResponseType(typeof(ComakershipComplete), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComakershipComplete([HttpTrigger(AuthorizationLevel.User, "get", Route = "comakerships/{id:int}/complete")] HttpRequest req, int id)
        {
            _logger.LogInformation("Getting comakership(complete): " + id);

            var comakership = await _comakershipService.GetComakershipComplete(id);
            if (comakership != null)
            {                
                return new OkObjectResult(comakership);
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Finds Comakerships by search term on the required Skills
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [QueryStringParameter("Skill", "Search for comakerships who require this skill", DataType = typeof(string), Required = true)]
        [FunctionName("FindComakershipsBySkill")]
        [ProducesResponseType(typeof(ComakershipBasic), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindComakershipsBySkill([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "comakerships/search")] HttpRequest req)
        {
            _logger.LogInformation("Searching comakerships");

            string skill;
            if (!string.IsNullOrEmpty(req.Query["skill"]))
            {
                skill = req.Query["skill"];
                var comakerships = await _comakershipService.FindComakershipsBySkill(skill);
                if (comakerships.Count > 0)
                {                   
                    return new OkObjectResult(comakerships);
                }
                return new NotFoundResult();
            }
            return new BadRequestObjectResult("Need to pass a term to search for");            
        }

        /// <summary>
        /// Create a new Comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("AddComakership")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddComakership([HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships")]
            [RequestBodyType(typeof(ComakershipPost), "The comakership to store")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            _logger.LogInformation("Adding comakership");

            var identity = (ClaimsIdentity)User.Identity;
            var usertype = identity.FindFirst("UserType").Value;

            // early return if logged in user is not a companyuser
            if (usertype != "CompanyUser")
            {
                return new UnauthorizedResult();
            }

            try
            {
                var newComakership = await JsonSerializer.DeserializeAsync<ComakershipPost>(req.Body, _options);
                try
                {
                    int id = await _comakershipService.CreateComakership(newComakership, identity);
                    return new OkObjectResult($"Comakership with id '{id}' added") { StatusCode = StatusCodes.Status201Created };
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

        /// <summary>
        /// Edits a Comakership
        /// </summary>
        /// <param name="req">req</param>
        /// <param name="id">Integer</param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("EditComakership")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditComakership([HttpTrigger(AuthorizationLevel.User, "put", Route = "comakerships/{id:int}")]
                                                [RequestBodyType(typeof(ComakershipPut), "The comakership to edit")] HttpRequest req, int id,
                                                [SwaggerIgnore] ClaimsPrincipal User)
        {
            _logger.LogInformation("Editting 'comakership' entity");

            var identity = (ClaimsIdentity)User.Identity;
            var usertype = identity.FindFirst("UserType").Value;

            // early return if logged in user is not a companyuser
            if (usertype != "CompanyUser")
            {
                return new UnauthorizedResult();
            }

            try
            {
                var putData = await JsonSerializer.DeserializeAsync<ComakershipPut>(req.Body, _options);

                if (putData.Id == id)
                {
                    if (await _comakershipService.UpdateComakership(putData, identity))
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
        /// Accepts the application for a comakership of a team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="comakershipid">Integer</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("AcceptApplication")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AcceptApplication(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships/{comakershipid:int}/acceptapplication/{teamid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User,
            int comakershipid,
            int teamid) {

            _logger.LogInformation("Endpoint 'AcceptApplication' is reached.");

            var identity = (ClaimsIdentity)User.Identity;
            var usertype = identity.FindFirst("UserType").Value;

            // early return if logged in user is not a companyuser
            if (usertype != "CompanyUser")
            {
                return new UnauthorizedResult();
            }

            try {
                if (await _comakershipService.AcceptApplication(teamid, comakershipid, identity)){
                    return new OkObjectResult("Sucessfully accepted application");
                } else {
                    return new ObjectResult("something went wrong processing your request, try again later") { StatusCode = 500 };
                }

            } catch (UnauthorizedAccessException e) {
                return new UnauthorizedObjectResult(e.Message);
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Rejects the application for a comakership of a team
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="comakershipid">Integer</param>
        /// <param name="teamid">Integer</param>
        /// <returns></returns>
        [FunctionName("RejectApplication")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RejectApplication(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships/{comakershipid:int}/rejectapplication/{teamid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User,
            int comakershipid,
            int teamid)
        {
            _logger.LogInformation("Endpoint 'RejectApplication' is reached.");

            var identity = (ClaimsIdentity)User.Identity;
            var usertype = identity.FindFirst("UserType").Value;

            // early return if logged in user is not a companyuser
            if (usertype != "CompanyUser")
            {
                return new UnauthorizedResult();
            }

            try
            {
                if (await _comakershipService.RejectApplication(teamid, comakershipid, identity))
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
        /// gets the comakerships of the currently logged in user
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("GetUserComakerships")]
        [ProducesResponseType(typeof(List<ComakershipComplete>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserComakerships(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "comakerships/loggedinuser/all")] HttpRequest req, //appended /all to route to circumvent dodgy route priority in azure functions
            [SwaggerIgnore] ClaimsPrincipal User) {

            _logger.LogInformation("Returning comakerships");

            var identity = (ClaimsIdentity)User.Identity;

            var comakerships = await _comakershipService.GetComakershipsForUser(identity);
            
            if(comakerships != null) {
                return new OkObjectResult(comakerships);
            }

            return new NotFoundObjectResult("No comakerships found");
        }


        /// <summary>
        /// Finds applications for a comakership
        /// </summary>
        /// <param name="id">Integer</param>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <returns></returns>
        [FunctionName("GetComakershipApplications")]
        [ProducesResponseType(typeof(List<TeamComakershipTeamGet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComakershipApplications([HttpTrigger(AuthorizationLevel.User, "get", Route = "comakerships/{id:int}/applications")] HttpRequest req, int id,
            [SwaggerIgnore] ClaimsPrincipal User)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var usertype = identity.FindFirst("UserType").Value;

            // early return if logged in user is not a companyuser
            if (usertype != "CompanyUser")
            {
                return new UnauthorizedResult();
            }

            var applications = await _comakershipService.GetApplications(id, identity);
            return new OkObjectResult(applications);
        }

        /// <summary>
        /// allows a user to leave a comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("LeaveComakership")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LeaveComakership(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships/{id:int}/leave")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int id)
        {
            _logger.LogInformation("Endpoint 'LeaveComakership' is reached.");

            var identity = (ClaimsIdentity)User.Identity;
            try
            {
                if (await _comakershipService.LeaveComakership(id, identity))
                {
                    return new OkObjectResult("Left the comakership");
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
        /// allows a companyuser to remove a student from a comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User">ClaimsPrincipal</param>
        /// <param name="id">Integer</param>
        /// <param name="studentid">Integer</param>
        /// <returns></returns>
        [FunctionName("RemoveStudentFromComakership")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveStudentFromComakership(
            [HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships/{id:int}/kick/{studentid:int}")] HttpRequest req,
            [SwaggerIgnore] ClaimsPrincipal User, int id, int studentid)
        {
            _logger.LogInformation("Endpoint 'KickStudent' is reached.");

            var identity = (ClaimsIdentity)User.Identity;
            var usertype = identity.FindFirst("UserType").Value;

            // early return if logged in user is not a companyuser
            if (usertype != "CompanyUser")
            {
                return new UnauthorizedResult();
            }

            try
            {
                if (await _comakershipService.KickStudent(id, studentid, identity))
                {
                    return new OkObjectResult("Removed from the comakership");
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
