using AzureFunctions.Extensions.Swashbuckle.Attribute;
using ComakershipsApi.Services;
using Dahomey.Json;
using Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceLayer.User;
using System.Linq;

namespace ComakershipsApi {
    public class LoginController {
        private readonly IUserService userService;

        ITokenService TokenService { get; }

        public LoginController(ITokenService TokenService, IUserService userService) {
            this.TokenService = TokenService;
            this.userService = userService;
        }

        /// <summary>
        /// Generates a token for any supplied login
        /// </summary>
        /// <returns></returns>
        [FunctionName("Login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
                [RequestBodyType(typeof(Login), "Credentials")] HttpRequest req) {

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.SetupExtensions();
            options.PropertyNameCaseInsensitive = true;

            Login login = await JsonSerializer.DeserializeAsync<Login>(req.Body, options);

            if (await userService.IsValidLogin(login.Email, login.Password)) {
                Security.Token Token = await TokenService.CreateToken(login, await userService.GetSingle(u => u.Email == login.Email));

                return new OkObjectResult(new Dictionary<string, string> {
                    { "UserType", Token.Claims.First(c => c.Type == "UserType").Value },
                    { "UserId", Token.Claims.First(c => c.Type == "UserId").Value },
                    { "Token", Token.Value }
                });
            }

            return new UnauthorizedObjectResult("Invalid Credentials");
        }
    }
}
