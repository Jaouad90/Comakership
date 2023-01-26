using AutoMapper;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using ComakershipsApi;
using Dahomey.Json;
using DAL;
using Models;
using Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using ServiceLayer;
using ServiceLayer.User;
using System.Linq;
using ComakershipsApi.Utils;
using Newtonsoft.Json.Serialization;

namespace ComakershipsApi {
    /// <summary>
    /// class definition for the UserController endpoint. It retrieves everything related to users.
    /// </summary>
    public class UserController {
        private readonly IUserService userService;
        private readonly ITeamService teamService;
        private readonly IMapper mapper;

        ILogger Logger { get; }

        JsonSerializerOptions options;

        public UserController(ILogger<UserController> Logger, IUserService userService, IMapper mapper, ITeamService teamService) {
            this.Logger = Logger;
            this.userService = userService;
            this.mapper = mapper;
            this.teamService = teamService;
            this.options = new JsonSerializerOptions() {
                PropertyNameCaseInsensitive = true
            }.SetupExtensions();
        }

        /// <summary>
		/// Retrieves a single student based on it's ID.
		/// </summary>
        /// <param name="id"></param>
		/// <returns></returns>
        [FunctionName("StudentGet")]
        [ProducesResponseType(typeof(StudentUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StudentGet(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "Students/{id}")] HttpRequest req, int id = 0) {

            Logger.LogInformation("C# [HTTP] User endpoint StudentGet has been triggered");

            StudentUser user = (StudentUser)await userService.GetUser<StudentUser>(id);

            //use newtonsoft json because system.text.json sucks at serializing
            Newtonsoft.Json.JsonSerializerSettings options = new Newtonsoft.Json.JsonSerializerSettings() {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (user != null) {

                if(user.Reviews != null) {
                    user.Reviews = user.Reviews.Where(r => !r.ForCompany);
                }
                
                // get company for each review

                return new JsonResult(user, options);
            }

            return new NotFoundObjectResult("User with this id not found");
        }

        /// <summary>
		/// Saves a new student
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
        [FunctionName("StudentPost")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StudentPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Students")]
            [RequestBodyType(typeof(StudentPostVM), "StudentUser to create")] HttpRequest req) {

            Logger.LogInformation("C# [HTTP] User endpoint StudentPost has been triggered");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            try {
                StudentPostVM user = JsonSerializer.Deserialize<StudentPostVM>(requestBody, options);
                // Prepare private team for student
                TeamPost newteam = new TeamPost
                {
                    Name = user.FirstName + " " + user.LastName,
                    Description = "This is my private team"
                };
                if (await userService.SaveUser<StudentUser>(mapper.Map<StudentUser>(user))) {

                    StudentUser newUser = mapper.Map<StudentUser>(await userService.GetSingle(u => u.Email == user.Email));
                    newUser.PrivateTeamId = await teamService.CreateTeam(newteam, newUser.Id);
                    if (await userService.EditUser<StudentUser>(newUser, newUser.Id)) //TODO rewrite to use EditStudentUser
                    {
                        return new ObjectResult($"New StudentUser Created with PrivateTeamId '{newUser.PrivateTeamId}'.") { StatusCode = StatusCodes.Status201Created };
                    }
                    else
                    {
                        return new ObjectResult("New StudentUser created, error occured during team creation") { StatusCode = StatusCodes.Status206PartialContent};
                    }
                } 

                return new BadRequestObjectResult("Passed object not valid");
            } catch (Exception e) {
                Logger.LogError(e.Message);

                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
		/// Edit a student
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
        [FunctionName("StudentPut")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> StudentPut(
            [HttpTrigger(AuthorizationLevel.User, "put", Route = "Students")]
            [RequestBodyType(typeof(StudentPutVM), "StudentUser to edit")] HttpRequest req, [SwaggerIgnore] ClaimsPrincipal User) {

            Logger.LogInformation("C# [HTTP] User endpoint StudentPut has been triggered");

            var identity = (ClaimsIdentity)User.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                StudentPutVM user = JsonSerializer.Deserialize<StudentPutVM>(requestBody, options);

                if (await userService.EditStudentUser(user, identity)) {
                    return new OkObjectResult("student edited") { StatusCode = StatusCodes.Status200OK };
                }

                return new BadRequestObjectResult("Passed object not valid");
            } catch (UnauthorizedAccessException e) {
                Logger.LogError(e.Message);

                return new UnauthorizedObjectResult(e.Message);
            } catch (Exception e) {
                Logger.LogError(e.Message);

                return new BadRequestObjectResult("Passed object not valid");
            }
        }

        /// <summary>
        /// Retrieves a single companyuser based on it's ID.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("CompanyUserGet")]
        [ProducesResponseType(typeof(CompanyUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyUserGet(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "CompanyUser/{id}")] HttpRequest req, int id = 0) {

            Logger.LogInformation("C# [HTTP] User endpoint CompanyUserGet has been triggered");

            var user = await userService.GetUser<CompanyUser>(id);

            if(user != null) {
                return new JsonResult(user);
            }

            return new NotFoundObjectResult("User with this id not found");
        }

        /// <summary>
		/// Saves a new companyuser
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
        [FunctionName("CompanyUserPost")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompanyUserPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CompanyUser")]
            [RequestBodyType(typeof(CompanyUserPostVM), "CompanyUser to create")] HttpRequest req) {

            Logger.LogInformation("C# [HTTP] User endpoint CompanyUserPost has been triggered");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                CompanyUserPostVM user = JsonSerializer.Deserialize<CompanyUserPostVM>(requestBody, options);

                if (await userService.SaveUser<CompanyUser>(mapper.Map<CompanyUser>(user))) {
                    return new ObjectResult("New CompanyUser Created") { StatusCode = StatusCodes.Status201Created };
                }

                return new BadRequestObjectResult("Passed object not valid");
            } catch (Exception e) {
                Logger.LogError(e.Message);

                return new BadRequestObjectResult("Passed object not valid");
            }
        }

        /// <summary>
		/// Edit a companyuser
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
        [FunctionName("CompanyUserPut")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompanyUserPut(
            [HttpTrigger(AuthorizationLevel.User, "put", Route = "CompanyUser")]
            [RequestBodyType(typeof(CompanyUserPutVM), "CompanyUser to edit")] HttpRequest req, [SwaggerIgnore] ClaimsPrincipal User) {

            Logger.LogInformation("C# [HTTP] User endpoint CompanyUserPut has been triggered");

            var identity = (ClaimsIdentity)User.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                CompanyUserPutVM user = JsonSerializer.Deserialize<CompanyUserPutVM>(requestBody, options);

                if(await userService.EditCompanyUser(user, identity)) {
                    return new OkObjectResult("CompanyUser edited") { StatusCode = StatusCodes.Status200OK };
                }

                return new BadRequestObjectResult("Passed object not valid");
            } catch (UnauthorizedAccessException e) {
                Logger.LogError(e.Message);

                return new UnauthorizedObjectResult(e.Message);
            } catch (Exception e) {
                Logger.LogError(e.Message);

                return new BadRequestObjectResult("Passed object not valid");
            }
        }

        /// <summary>
		/// Deletes a single student based on it's ID.
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
        [FunctionName("StudentDelete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> StudentDelete(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = "Students/{id}")] HttpRequest req, [SwaggerIgnore] ClaimsPrincipal User, int id = 0) {

            Logger.LogInformation("C# [HTTP] User endpoint StudentDelete has been triggered");

            var identity = (ClaimsIdentity)User.Identity;

            try {
                if(await userService.DeleteUser<StudentUser>(id, identity)) {
                    return new OkObjectResult("Student was successfully deleted");
                }
                return new BadRequestObjectResult("Failed to delete user");
            } catch (UnauthorizedAccessException e) {
                Logger.LogError(e.Message);

                return new UnauthorizedObjectResult(e.Message);
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
		/// Deletes a single companyuser based on it's ID.
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
        [FunctionName("CompanyUserDelete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompanyUserDelete(
            [HttpTrigger(AuthorizationLevel.User, "delete", Route = "CompanyUser/{id}")] HttpRequest req, [SwaggerIgnore] ClaimsPrincipal User, int id = 0) {

            Logger.LogInformation("C# [HTTP] User endpoint CompanyUserDelete has been triggered");

            var identity = (ClaimsIdentity)User.Identity;

            try {
                if(await userService.DeleteUser<CompanyUser>(id, identity)) {
                    return new OkObjectResult("Company user was successfully deleted");
                }
                return new BadRequestObjectResult("Failed to delete user");
            } catch (UnauthorizedAccessException e) {
                Logger.LogError(e.Message);

                return new UnauthorizedObjectResult(e.Message);
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Changes the logged in users' password
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("ChangePassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword(
             [HttpTrigger(AuthorizationLevel.User, "post", Route = "User/ChangePassword")]
             [RequestBodyType(typeof(ChangePasswordVM), "Password object")] HttpRequest req, 
             [SwaggerIgnore] ClaimsPrincipal User
            ) {

            Logger.LogInformation("C# [HTTP] User endpoint ChangePassword has been triggered");

            var identity = (ClaimsIdentity)User.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                ChangePasswordVM pass = JsonSerializer.Deserialize<ChangePasswordVM>(requestBody, options);

                if (await userService.ChangePassword(pass.OldPassword, pass.NewPassword, pass.ConfirmNewPassword, identity)) {
                    return new OkObjectResult("Successfully changed password");
                }

                return new BadRequestObjectResult("Failed to change password, check your input and try again");
            } catch(Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Add a new skill to the logged in student
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        [FunctionName("AddSkill")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddSkill(
             [HttpTrigger(AuthorizationLevel.User, "post", Route = "User/AddSkill")]
             [RequestBodyType(typeof(string), "Skill name")] HttpRequest req,
             [SwaggerIgnore] ClaimsPrincipal User
            ) {

            Logger.LogInformation("C# [HTTP] User endpoint AddSkill has been triggered");

            var identity = (ClaimsIdentity)User.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                string skill = JsonSerializer.Deserialize<string>(requestBody, options);

                if (await userService.AddSkill(skill, identity)) {
                    return new OkObjectResult("Successfully added skill");
                }

                return new BadRequestObjectResult("Failed to add skill, check your input and try again");
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Delete a skill from logged in student
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        [FunctionName("DeleteSkill")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSkill (
             [HttpTrigger(AuthorizationLevel.User, "delete", Route = "User/DeleteSkill")]
             [RequestBodyType(typeof(string), "Skill name")] HttpRequest req,
             [SwaggerIgnore] ClaimsPrincipal User
            ) {

            Logger.LogInformation("C# [HTTP] User endpoint DeleteSkill has been triggered");

            var identity = (ClaimsIdentity)User.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                string skill = JsonSerializer.Deserialize<string>(requestBody, options);

                if (await userService.DeleteSkill(skill, identity)) {
                    return new OkObjectResult("Successfully deleted skill");
                }

                return new BadRequestObjectResult("Failed to delete skill, check your input and try again");
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// Edit a skill from logged in student
        /// </summary>
        /// <param name="req"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        [FunctionName("EditSkill")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditSkill(
             [HttpTrigger(AuthorizationLevel.User, "put", Route = "User/EditSkill")]
             [RequestBodyType(typeof(EditSkillVM), "Skill name")] HttpRequest req,
             [SwaggerIgnore] ClaimsPrincipal User
            ) {

            Logger.LogInformation("C# [HTTP] User endpoint EditSkill has been triggered");

            var identity = (ClaimsIdentity)User.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try {
                EditSkillVM skills = JsonSerializer.Deserialize<EditSkillVM>(requestBody, options);

                if (await userService.EditSkill(skills.OldSkill, skills.NewSkill, identity)) {
                    return new OkObjectResult("Successfully edited skill");
                }

                return new BadRequestObjectResult("Failed to edit skill, check your input and try again");
            } catch (Exception e) {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
