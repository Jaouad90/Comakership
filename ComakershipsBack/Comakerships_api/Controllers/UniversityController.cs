using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using System.Text.Json;
using Dahomey.Json;
using Models;
using Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using DAL;
using Service;

namespace ComakershipsApi
{
    /// <summary>
    /// class definition for the UniversityController endpoint. It retrieves everything related to universities.
    /// </summary>
    public class UniversityController
    {
        private readonly IMapper _mapper;
        private IUniversityService _universityService;
        private JsonSerializerOptions _JsonSerializerOptions;
        ILogger _Logger { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="universityService"></param>
        /// <param name="mapper"></param>
        public UniversityController(ILogger<UniversityController> logger, IUniversityService universityService, IMapper mapper)
        {
            _Logger = logger;
            _mapper = mapper;
            _universityService = universityService;

            _JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }.SetupExtensions();

        }


        /// <summary>
        /// This method allows you to retrieve all universities.
        /// </summary>
        /// <param name="req"></param>
        /// <response code="200">Is returned when a university is successfully retrieved.</response>
        [FunctionName("UniversitiesGet")]
        [ProducesResponseType(typeof(University), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UniversitiesGet([HttpTrigger(AuthorizationLevel.User, "get", Route = "universities")] HttpRequest req)
        {

            _Logger.LogInformation("C# [HTTP] Universities endpoint has been triggered with a GET request.");

            IEnumerable<University> universitiesList = await _universityService.GetAllUniversitiesAsync();
            return new OkObjectResult(universitiesList);
        }

        /// <summary>
        /// This method allows you to retrieve all email domains of all universities.
        /// </summary>
        /// <param name="req"></param>
        /// <response code="200">Is returned when the domains are succesfully retrieved.</response>
        [FunctionName("UniversitiesDomainsGet")]
        [ProducesResponseType(typeof(IEnumerable<UniversityDomainVM>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UniversitiesDomainsGet([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "universities/domains")] HttpRequest req)
        {

            _Logger.LogInformation("C# [HTTP] Universities/domains endpoint has been triggered with a GET request.");

            IEnumerable<UniversityDomainVM> universityDomainList = await _universityService.GetAllUniversityDomainVMAsync();


            return new OkObjectResult(universityDomainList);
        }

        /// <summary>
        /// This method allows you to retrieve the domain used by the university.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Is returned when the domain is succesfully retrieved.</response>
        /// <response code="404">Is returned when the requested university doesn't exist.</response>
        [FunctionName("UniversityDomainGet")]
        [ProducesResponseType(typeof(UniversityDomainVM), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UniversitiesDomainGetById([HttpTrigger(AuthorizationLevel.User, "get", Route = "universities/{id}/domain")] HttpRequest req, int id)
        {

            _Logger.LogInformation("C# [HTTP] Universities/domains endpoint has been triggered with a GET request.");

            if (await _universityService.CheckIfUniversityExistsAsync(id))
            {

                UniversityDomainVM universityDomain = await _universityService.GetUniversityDomainByIdAsync(id);
                return new OkObjectResult(universityDomain);
            }
            responseBody notFoundBody = new responseBody
            {
                StatusCode = 404,
                Message = "Requested university was not found."
            };
            return new NotFoundObjectResult(notFoundBody);
        }

        /// <summary>
        /// This method allows you to retrieve a single university based on it's ID.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <response code="200">Is returned when a university is successfully retrieved.</response>
        /// <response code="404">Is returned when the requested university doesn't exist.</response>
        [FunctionName("UniversityGet")]
        [ProducesResponseType(typeof(University), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UniversityGet([HttpTrigger(AuthorizationLevel.User, "get", Route = "university/{id}")] HttpRequest req, int id = 0)
        {
            _Logger.LogInformation("C# [HTTP] University endpoint has been triggered with a GET request.");

            if (await _universityService.CheckIfUniversityExistsAsync(id))
            {
                University university = await _universityService.GetUniversityByIdAsync(id);
                return new OkObjectResult(university);
            }
            else
            {
                responseBody notFoundBody = new responseBody
                {
                    StatusCode = 404,
                    Message = "Requested university was not found."
                };
                return new NotFoundObjectResult(notFoundBody);
            }
        }

		/// <summary>
		/// This method allows you to add a new university to the Comakerships App.
		/// </summary>
		/// <remarks>
		/// Sample request to create an university:
		///
		///     POST /api/University
		///		{
		///		 "id": 66,
		///		  "name": "Inholland",
		///		  "location": {
		///			"street": "Teststreet 13",
		///			"city": "Testcity",
		///			"zipcode": "1834TEST"
		///		  },
		///		  "registrationDate": "2020-09-20T14:05:08.5440000+02:00",
		///		  "domain":"student.inholland.nl"
		///		}
		/// </remarks>
		/// <param name="req"></param>
		/// <param name="queue"></param>
		/// <returns></returns>
		/// <response code="200">Is returned when a university is successfully added.</response>
		/// <response code="400">Is returned when the supplied university object is not valid.</response>
		/// <response code="409">Is returned when the supplied university object already exists.</response>
		[FunctionName("UniversityPost")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
		public async Task<IActionResult> UniversityPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "university")]
												     [RequestBodyType(typeof(University), "The university to be created")] HttpRequest req,
													 [Queue("UniversityPostQueue")] IAsyncCollector<UniversityPostVM> queue)
		{
            _Logger.LogInformation("C# [HTTP] University endpoint has been triggered with a POST request.");

            try
            {
                UniversityPostVM universityPostVM = await JsonSerializer.DeserializeAsync<UniversityPostVM>(req.Body, _JsonSerializerOptions);

                if (universityPostVM != null)
                {
                    if (!await _universityService.CheckIfUniversitynameExistsAsync(universityPostVM.Name))
                    {
                        await queue.AddAsync(universityPostVM);

                        responseBody OkBody = new responseBody()
                        {
                            StatusCode = 200,
                            Message = $"New university has been queued for creation: {universityPostVM.Name}"
                        };
                        return new OkObjectResult(OkBody);
                    }
                    else
                    {
                        responseBody ConflictBody = new responseBody()
                        {
                            StatusCode = 409,
                            Message = $"Given name already exists: {universityPostVM.Name}"
                        };
                        return new ConflictObjectResult(ConflictBody);
                    }
                }
                else return new BadRequestObjectResult("You should send an object");
            }
            catch (JsonException e)
            {
                responseBody badRequestBody = new responseBody()
                {
                    StatusCode = 400,
                    Message = e.Message
                };
                return new BadRequestObjectResult(badRequestBody);
            }
        }

        /// <summary>
        /// This is the function that's responsible for handling the messages that are added to the University Delete Queue.
        /// </summary>
        /// <param name="message"></param>
        [FunctionName("UniversityPostQueueTrigger")]
        public async Task UniversityPostQueueTriggerAsync([QueueTrigger("UniversityPostQueue")] string message)
        {
            try
            {
                UniversityPostVM universityPostVM = JsonSerializer.Deserialize<UniversityPostVM>(message, _JsonSerializerOptions);

                await _universityService.SaveUniversityAsync(universityPostVM);
                _Logger.LogInformation($"The following university has been created: {universityPostVM.Name}");
            }
            catch (JsonException e)
            {
                _Logger.LogInformation($"error occured: {e}");
            }
        }

        /// <summary>
        /// This method allows you to delete one university based on it's ID.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="queue"></param>
        /// <response code="200">Is returned when a university is successfully deleted.</response>
        /// <response code="202">Is returned when a request has been succesfully queued but not yet processed.</response>
        /// <response code="404">Is returned when the university requested to be deleted isn't found.</response>
        [FunctionName("UniversityDelete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UniversityDelete([HttpTrigger(AuthorizationLevel.User, "delete", Route = "university/{id}")] HttpRequest req, int id,
                                                          [Queue("UniversityDeleteQueue")] IAsyncCollector<UniversityDeleteVM> queue)
        {
            _Logger.LogInformation("C# [HTTP] University endpoint has been triggered with a DELETE request.");

            if (await _universityService.CheckIfUniversityExistsAsync(id))
            {
                UniversityDeleteVM universityDeleteVM = new UniversityDeleteVM(id);

                await queue.AddAsync(universityDeleteVM);

                responseBody OkBody = new responseBody()
                {
                    StatusCode = 202,
                    Message = $"Your request has been accepted and is queued."
                };
                return new OkObjectResult(OkBody);
            }
            else
            {
                responseBody notFoundBody = new responseBody
                {
                    StatusCode = 404,
                    Message = "Requested university was not found."
                };
                return new NotFoundObjectResult(notFoundBody);
            }
        }

        /// <summary>
        /// This is the function that's responsible for handling the messages that are added to the University Delete Queue.
        /// </summary>
        /// <param name="message"></param>
        [FunctionName("UniversityDeleteQueueTrigger")]
        public async Task UniversityDeleteQueueTriggerAsync([QueueTrigger("UniversityDeleteQueue")] string message)
        {
            try
            {
                UniversityDeleteVM universityDeleteVM = JsonSerializer.Deserialize<UniversityDeleteVM>(message, _JsonSerializerOptions);

                await _universityService.DeleteUniversityByIdAsync(universityDeleteVM.Id);               // TODO write implementation where the object is actually deleted from the DB with an AWAIT statement.
                _Logger.LogInformation($"The university has been deleted.");
            }
            catch (JsonException e)
            {
                _Logger.LogInformation($"error occured: {e}");
            }
        }

        /// <summary>
        /// This method allows you to update some fields or everything on an university object.
        /// </summary>
        /// <remarks>
        /// Sample request to modify an university:
        ///
        ///     PUT /api/university/66
        ///		{
        ///		  "name": "InhollandModifiedName",
        ///		  "city": "NewCity",
        ///		}
        /// </remarks>
        /// <param name="req"></param>
        /// <param name="queue"></param>
        /// <param name="id"></param>
        /// <response code="200">Is returned when a valid university object is supplied and is successfully modified.</response>
        /// <response code="400">Is returned when a non valid university object is supplied.</response>
        [FunctionName("UniversityPut")]
        [ProducesResponseType(typeof(University), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UniversityPut([HttpTrigger(AuthorizationLevel.User, "put", Route = "university/{id}")]
                                                       [RequestBodyType(typeof(UniversityPutVM), "The university to be edited")] HttpRequest req, int? id,
                                                       [Queue("UniversityPutQueue")] IAsyncCollector<UniversityPutVM> queue)
        {
            _Logger.LogInformation("C# [HTTP] University endpoint has been triggered with a PUT request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                _JsonSerializerOptions.IgnoreNullValues = true;

                UniversityPutVM universityPutVM = JsonSerializer.Deserialize<UniversityPutVM>(requestBody, _JsonSerializerOptions);

                if (universityPutVM.Id == null)
                {
                    universityPutVM.Id = id;
                }
                else
                {
                    if (universityPutVM.Id != id)
                    {
                        responseBody BadRequestBody = new responseBody
                        {
                            StatusCode = 400,
                            Message = $"Path parameter: {id} is not the same as supplied Id in object: {universityPutVM.Id}. Emit Id from given object."
                        };
                        return new NotFoundObjectResult(BadRequestBody);
                    }
                }

                if (await _universityService.CheckIfUniversityExistsAsync((int)id))
                {
                    await queue.AddAsync(universityPutVM);

                    responseBody OkResponseBody = new responseBody
                    {
                        StatusCode = 200,
                        Message = "Modifications to university where succesfully added to queue."
                    };
                    return new OkObjectResult(OkResponseBody);
                }
                else
                {
                    responseBody NotFoundBody = new responseBody
                    {
                        StatusCode = 404,
                        Message = "The university was not found."
                    };
                    return new OkObjectResult(NotFoundBody);
                }


                
            }
            catch (JsonException e)
            {
                _Logger.LogError(e.Message);
                responseBody BadRequestBody = new responseBody
                {
                    StatusCode = 400,
                    Message = e.Message
                };
                return new NotFoundObjectResult(BadRequestBody);
            }
        }

        /// <summary>
        /// This is the function that's responsible for handling the messages that are added to the University Put Queue.
        /// </summary>
        /// <param name="message"></param>
        [FunctionName("UniversityPutQueueTrigger")]
        public async Task UniversityPutQueueTriggerAsync([QueueTrigger("UniversityPutQueue")] string message)
        {
            try
            {
                UniversityPutVM universityPutVM = JsonSerializer.Deserialize<UniversityPutVM>(message, _JsonSerializerOptions);

                await _universityService.EditUniversityAsync(universityPutVM);
                // TODO write implementation where the object is actually modified from the DB with an AWAIT statement.
                _Logger.LogInformation($"The following university has been modified: {universityPutVM.Name}");
            }
            catch (JsonException e)
            {
                _Logger.LogInformation($"error occured: {e}");
            }
        }
    }
}