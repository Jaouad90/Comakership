using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Dahomey.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Models;
using Models.ViewModels;
using Service;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ComakershipsApi.Controllers
{
    /// <summary>
    /// class definition for the ReviewController. It contains endpoints related to Reviews.
    /// </summary>
    class ReviewController
    {
        ILogger Logger { get; }
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Constructor for the ComakershipController
        /// </summary>
        public ReviewController(ILogger<ReviewController> Logger, IReviewService reviewService)
        {
            this.Logger = Logger;
            _reviewService = reviewService;
        }

        /// <summary>
        /// This method allows you to add a new review for a company
        /// </summary>
        /// <remarks>
        /// Sample request to create a review without:
        ///
        ///     POST /api/review
        ///     {
        ///       "CompanyId" : 3,
        ///       "StudentUserId" : 1,
        ///       "Rating" : 8,
        ///       "Comment" : "Dit bedrijf is top!",
        ///       "ForCompany" : true
        ///     }
        /// </remarks>
        /// <param name="req"></param>
        /// <response code="200">Is returned when a review is successfully added.</response>
        /// <response code="400">Is returned when the supplied review object is not valid. </response>
        [FunctionName("ReviewPost")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReviewPost([HttpTrigger(AuthorizationLevel.User, "post", Route = "review")]
                                                   [RequestBodyType(typeof(ReviewPostVM), "The review to be created")] HttpRequest req, [SwaggerIgnore] ClaimsPrincipal User)
        {
            Logger.LogInformation("C# [HTTP] Review endpoint has been triggered with a POST request.");

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.SetupExtensions();
                options.PropertyNameCaseInsensitive = true;
                ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
                
                ReviewPostVM reviewPostVM = await JsonSerializer.DeserializeAsync<ReviewPostVM>(req.Body, options);

                // check if object is null and if logged in user is placing their own review 
                if (reviewPostVM != null && (
                    // If placed by studentUser, loggedin UserId has to match StudentId
                    ("StudentUser" == identity.FindFirst("UserType").Value && reviewPostVM.StudentUserId == int.Parse(identity.FindFirst("UserId").Value) && reviewPostVM.ForCompany) || 
                    // If placed by Company, CompanyId of logged in CompanyUser has to match CompanyId
                    ("StudentUser" != identity.FindFirst("UserType").Value && reviewPostVM.CompanyId == int.Parse(identity.FindFirst("CompanyId").Value) && !reviewPostVM.ForCompany)))
                {
                    int id = await _reviewService.SaveReview(reviewPostVM, identity);
                    return new ObjectResult($"Review with id '{id}' added") { StatusCode = StatusCodes.Status201Created};
                }
                else return new BadRequestObjectResult("Something went wrong, check your input and try again.") { StatusCode = StatusCodes.Status400BadRequest};
            }
            catch (Exception e) {
                return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }
    }
}

