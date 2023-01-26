using System;
using System.Collections.Generic;
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
using System.IO;
using AutoMapper;
using Models.ViewModels;
using Service;
using System.Security.Claims;

namespace ComakershipsApi
{
    /// <summary>
    /// class definition for the CompanyController endpoint. It retrieves everything related to companies.
    /// </summary>
    public class CompanyController
    {
        private readonly IMapper _mapper;
        private ICompanyService _companyService;
        private JsonSerializerOptions _JsonSerializerOptions;

        ILogger Logger { get; }

        /// <summary>
        /// Function that uses the ILogger interface for logging capabilities.
        /// </summary>
        /// <param name="Logger"></param>
        /// <param name="companyService"></param>
        /// <param name="mapper"></param>
        public CompanyController(ILogger<CompanyController> Logger, ICompanyService companyService, IMapper mapper)
        {
            this.Logger = Logger;
            _mapper = mapper;
            _companyService = companyService;

            _JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }.SetupExtensions();
        }

        /// <summary>
        /// This method allows you to retrieve a list of companies.
        /// </summary>
        /// <param name="req"></param>
        /// <response code="200">Is returned when the list of companies is successfully retrieved.</response>
        [FunctionName("CompaniesGet")]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<IActionResult> CompaniesGet([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies")]
                                                       HttpRequest req)
        {
            Logger.LogInformation("C# [HTTP] Companies endpoint has been triggered with a GET request.");

            IEnumerable<CompanyVM> companyList = await _companyService.GetAllCompaniesAsync();

            return new OkObjectResult(companyList) { StatusCode = StatusCodes.Status200OK };
        }

        /// <summary>
        /// This method allows you to retrieve a single company based on it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <response code="200">Is returned when a company is successfully retrieved.</response>
        /// <response code="404">Is returned when the requested company isn't found.</response>
        [FunctionName("CompanyGet")]
        [ProducesResponseType(typeof(CompanyVM), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyGet([HttpTrigger(AuthorizationLevel.User, "get", Route = "company/{id:int}")]
                                                     HttpRequest req, int id)
        {
            Logger.LogInformation("C# [HTTP] Company endpoint has been triggered with a GET request.");

            if (await _companyService.CheckifCompanyExistsAsync(id))
            {
                CompanyVM companyVM = await _companyService.GetCompanyByIdAsync(id);

                return new OkObjectResult(companyVM) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                return new NotFoundObjectResult("Requested company was not found.") { StatusCode = StatusCodes.Status404NotFound };
            }
        }

        /// <summary>
        /// This method allows you to retrieve a list of comakerships owned by the company, based on ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <response code="200">Is returned when the comakerships are successfully retrieved from the requested company.</response>
        /// <response code="404">Is returned when the requested company isn't found.</response>
        [FunctionName("CompanyComakershipsGet")]
        [ProducesResponseType(typeof(ComakershipComplete), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyComakershipsGet([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "company/{id:int}/comakerships")]
                                                                 HttpRequest req, int id)
        {
            Logger.LogInformation("C# [HTTP] Company/comakerships endpoint has been triggered with a GET request.");

            if (await _companyService.CheckifCompanyExistsAsync(id))
            {
                try
                {
                    IEnumerable<ComakershipComplete> companyComakerships = await _companyService.GetAllComakershipsOfCompanyByCompanyId(id);
                    return new OkObjectResult(companyComakerships) { StatusCode = StatusCodes.Status200OK };
                }
                catch (Exception e)
                {
                    return new NotFoundObjectResult(e.Message) { StatusCode = StatusCodes.Status404NotFound }; ;
                }
            }
            else
            {
                return new NotFoundObjectResult("Requested company was not found.") { StatusCode = StatusCodes.Status404NotFound }; ;
            }
        }

        /// <summary>
        /// This method allows you to retrieve the logo/image of the company, 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <response code="200">Is returned when the logo is successfully retrieved of the requested company.</response>
        /// <response code="404">Is returned when the company hasn't uploaded a custom logo/image yet.</response>
        [FunctionName("CompanyLogoGet")]
        [ProducesResponseType(typeof(Uri), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyLogoGet([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "company/{id:int}/logo")]
                                                         HttpRequest req, int id)
        {
            Logger.LogInformation("C# [HTTP] Company/logo endpoint has been triggered with a GET request.");

            if (await _companyService.CheckifCompanyExistsAsync(id))
            {
                try
                {
                    Uri uri = await _companyService.GetSASUriFromAzureBlobStorage(id);
                    return new OkObjectResult(uri) { StatusCode = StatusCodes.Status200OK };
                }
                catch (Exception e)
                {
                    return new NotFoundObjectResult(e.Message) { StatusCode = StatusCodes.Status404NotFound }; ;
                }
            }
            return new NotFoundObjectResult("Something went wrong, logo was not found.") { StatusCode = StatusCodes.Status404NotFound };
        }

        /// <summary>
        /// This method allows you to retrieve a list of registered employees.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <param name="user"></param>
        /// <response code="200">Is returned when the logo is successfully retrieved of the requested company.</response>
        [FunctionName("CompanyEmployeesGet")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyEmployeesGet([HttpTrigger(AuthorizationLevel.User, "get", Route = "company/{id:int}/employees")]
                                                              HttpRequest req, int id,
                                                             [SwaggerIgnore] ClaimsPrincipal user)
        {
            Logger.LogInformation("C# [HTTP] Company/employees endpoint has been triggered with a GET request.");
            var identity = (ClaimsIdentity)user.Identity;
            try
            {
                IEnumerable<CompanyUserVM> companyUsers = await _companyService.GetAllEmployeesAsync(id, identity);
                return new OkObjectResult(companyUsers) { StatusCode = StatusCodes.Status200OK };
            }
            catch (UnauthorizedAccessException e)
            {
                return new UnauthorizedObjectResult(e.Message) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Something went wrong, please try again.") { StatusCode = StatusCodes.Status400BadRequest };
                throw;
            }
        }

        /// <summary>
        /// This method allows you to post the logo/image of the company, 
        /// </summary>
        /// <remarks>
        /// Sample request to upload an image/logo for an company:
        ///
        ///     POST /api/company/{id}/logo
        ///     {
        ///       "LogoAsBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAE0lEQVR42mNkZPhfz4AGGGkgCADAFgeBun8CWgAAAABJRU5ErkJggg=="
        ///     }
        ///     
        ///   
        ///  </remarks>     
        /// <param name="id"></param>
        /// <param name="req"></param>
        /// <param name="user"></param>
        /// <response code="200">Is returned when the logo is successfully retrieved of the requested company.</response>
        /// <response code="404">Is returned when the company hasn't uploaded a custom logo/image yet.</response>
        [FunctionName("CompanyLogoPost")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompanyLogoPost([HttpTrigger(AuthorizationLevel.User, "post", Route = "company/{id:int}/logo")]
                                                         [RequestBodyType(typeof(CompanyLogoPostVM), "The logo to be uploaded")]
                                                          HttpRequest req, int id,
                                                         [SwaggerIgnore] ClaimsPrincipal user)
        {
            Logger.LogInformation("C# [HTTP] Company/logo endpoint has been triggered with a POST request.");
            var identity = (ClaimsIdentity)user.Identity;

            // not sure if this is the right way of whitelisting input. Even without this measurement you can't post anything else than a integer. All else fails with HTTP 500.
            if (id.GetType() == typeof(int))
            {
                try
                {
                    await _companyService.CheckifCompanyExistsAsync(id, identity);
                }
                catch (UnauthorizedAccessException e)
                {
                    return new UnauthorizedObjectResult(e.Message) { StatusCode = StatusCodes.Status401Unauthorized };
                }

                try
                {
                    CompanyLogoPostVM companyLogoPostVM = await JsonSerializer.DeserializeAsync<CompanyLogoPostVM>(req.Body, _JsonSerializerOptions);
                    CompanyLogoVM companyLogoVM = _mapper.Map<CompanyLogoVM>(companyLogoPostVM);
                    companyLogoVM.Id = id;

                    Guid guid = await _companyService.UploadToBlobStorage(companyLogoPostVM.LogoAsBase64, id);

                    if (guid != Guid.Empty)
                    {
                        companyLogoVM.LogoGuid = guid;
                        CompanyPutVM companyPutVM = _mapper.Map<CompanyPutVM>(companyLogoPostVM);

                        // need to do this in AM still, ask SDJ for help
                        companyPutVM.Id = id;
                        companyPutVM.LogoGuid = guid;

                        await _companyService.EditCompanyAsync(companyPutVM);

                        return new OkObjectResult("Photo has been uploaded to Azure storage.") { StatusCode = StatusCodes.Status200OK };
                    }
                    return new BadRequestObjectResult("Photo has been not been uploaded to Azure storage. Please try again.") { StatusCode = StatusCodes.Status400BadRequest };
                }
                catch (JsonException e)
                {
                    return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
                }
                catch (FormatException e)
                {
                    return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
                }
                catch (Exception e)
                {
                    return new NotFoundObjectResult(e.Message) { StatusCode = StatusCodes.Status404NotFound };
                }
            }
            return new BadRequestObjectResult("Supplied object malformed. please try again") { StatusCode = StatusCodes.Status400BadRequest };
        }


        /// <summary>
        /// This method allows you to add a new company to the Comakerships App.
        /// </summary>
        /// <remarks>
        /// Sample request to create an company without a logo:
        ///
        ///     POST /api/company
        ///     {
        ///       "name": "TestCompany",
        ///       "description": "Insert very interesting description here",
        ///       "street": "Teststreet 13",
        ///       "city": "Testcity",
        ///       "zipcode": "1834TEST",
        ///       "CompanyUser": {
        ///          "name": "User Example",
        ///          "email": "user@testcompany.com",
        ///          "password": "titapw"
        ///         }
        ///       }
        /// Sample request to create an company with a logo:
        ///
        ///     POST /api/company
        ///     {
        ///       "name": "TestCompany",
        ///       "description": "Insert very interesting description here",
        ///       "LogoAsBase64": "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAE0lEQVR42mNkZPhfz4AGGGkgCADAFgeBun8CWgAAAABJRU5ErkJggg==",
        ///       "street": "Teststreet 13",
        ///       "city": "Testcity",
        ///       "zipcode": "1834TEST",
        ///       "CompanyUser": {
        ///          "name": "User Example",
        ///          "email": "user@testcompany.com",
        ///          "password": "titapw"
        ///         }
        ///       }
        /// </remarks>
        /// <param name="req"></param>
        /// <response code="200">Is returned when a company is successfully added.</response>
        /// <response code="400">Is returned when the supplied company object is not valid. </response>
        /// <response code="409">Is returned when the supplied company has a name that already exists. </response>
        [FunctionName("CompanyPost")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompanyPost([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "company")]
                                                     [RequestBodyType(typeof(CompanyPostVM), "The company to be created")] HttpRequest req)
        {
            Logger.LogInformation("C# [HTTP] Company endpoint has been triggered with a POST request.");

            try
            {
                CompanyPostVM companyPostVM = await JsonSerializer.DeserializeAsync<CompanyPostVM>(req.Body, _JsonSerializerOptions);

                if (companyPostVM != null)
                {
                    if (!await _companyService.CheckIfCompanynameExistsAsync(companyPostVM.Name))
                    {
                        // setting Empty GUID, so that when there is no logo provided it'll default to 0000-00.. which can be translated to placeholder image.
                        Guid guid = Guid.Empty;

                        if (companyPostVM.LogoAsBase64 != null && companyPostVM.LogoAsBase64 != "")
                        {
                            try
                            {
                                guid = await _companyService.UploadToBlobStorage(companyPostVM.LogoAsBase64);
                            }
                            catch (Exception e)
                            {
                                return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
                            }

                        }
                        companyPostVM.LogoGuid = guid;
                        companyPostVM.LogoAsBase64 = null;

                        try
                        {
                            if (await _companyService.SaveCompanyAsync(companyPostVM))
                            {
                                if (companyPostVM.LogoGuid != Guid.Empty)
                                {
                                    var companyLogoVM = await _companyService.GetCompanyIdAndGuidByCompanyNameAsync(companyPostVM.Name);

                                    var moved = await _companyService.MoveCompanyLogoFromGenericStorageToCompanyStorage(companyLogoVM);
                                }
                                return new OkObjectResult($"New company has been created: {companyPostVM.Name}") { StatusCode = StatusCodes.Status201Created };
                            }
                            else
                            {
                                return new BadRequestObjectResult("Something went wrong, check your input and try again") { StatusCode = StatusCodes.Status400BadRequest };
                            }
                        }
                        catch (ArgumentException e)
                        {
                            return new ConflictObjectResult($"{e.Message}") { StatusCode = StatusCodes.Status409Conflict };
                        }
                    }
                    else
                    {
                        return new ConflictObjectResult($"Given name already exists: {companyPostVM.Name}") { StatusCode = StatusCodes.Status409Conflict };
                    }
                }
                else return new BadRequestObjectResult("You should send an object") { StatusCode = StatusCodes.Status400BadRequest }; ;
            }
            catch (JsonException e)
            {
                return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }

        /// <summary>
        /// This method allows you to delete one company based on the ID given in path.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="queue"></param>
        /// <param name="user"></param>
        /// <response code="200">Is returned when a company is successfully deleted.</response>
        /// <response code="404">Is returned when two of the querystring parameters are supplied, or no valid ones.</response>
        [FunctionName("CompanyDelete")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyDelete([HttpTrigger(AuthorizationLevel.User, "delete", Route = "company/{id:int}")]
                                                        HttpRequest req, int id,
                                                       [Queue("CompanyDeleteQueue")] IAsyncCollector<CompanyDeleteVM> queue,
                                                       [SwaggerIgnore] ClaimsPrincipal user)
        {
            Logger.LogInformation("C# [HTTP] Company endpoint has been triggered with a DELETE request.");
            var identity = (ClaimsIdentity)user.Identity;

            if (!await _companyService.CheckIfUserIsAuthorized(identity, id))
            {
                return new UnauthorizedObjectResult("Not authorized to delete the company.") { StatusCode = StatusCodes.Status401Unauthorized };
            }
            if (await _companyService.CheckifCompanyExistsAsync(id))
            {
                CompanyDeleteVM companyDeleteVM = new CompanyDeleteVM(id);
                await queue.AddAsync(companyDeleteVM);

                return new OkObjectResult("Your request has been accepted and is queued.") { StatusCode = StatusCodes.Status202Accepted };
            }
            else
            {
                return new NotFoundObjectResult("The company you're trying to delete does not exist.") { StatusCode = StatusCodes.Status404NotFound };
            }
        }

        /// <summary>
        /// This is the function that's responsible for handling the messages that are added to the Company Delete Queue.
        /// </summary>
        /// <param name="message"></param>
        [FunctionName("CompanyDeleteQueueTrigger")]
        public async Task CompanyDeleteQueueTriggerAsync([QueueTrigger("CompanyDeleteQueue")] string message)
        {
            try
            {
                CompanyDeleteVM CompanyDeleteVM = JsonSerializer.Deserialize<CompanyDeleteVM>(message, _JsonSerializerOptions);

                await _companyService.DeleteCompanyByIdAsync(CompanyDeleteVM.Id);
                Logger.LogInformation($"The company has been deleted.");
            }
            catch (JsonException e)
            {
                Logger.LogInformation($"error occured: {e}");
            }
        }

        /// <summary>
        /// This method allows you to update some fields or everything on an company object.
        /// </summary>
        /// <remarks>
        /// Sample request to modify the name of an company:
        ///
        ///     POST /api/company/{id}
        ///     {
        ///       "name": "NewCompanyName"
        ///      }
        ///      
        /// Sample request to modify the address of an company:
        ///
        ///     POST /api/company/{id}
        ///     {
        ///       "street": "Newstreet 25",
        ///       "city": "Newcity",
        ///       "zipcode": "1834NEW"
        ///      }
        ///      
        /// Sample request to modify the description of an company:
        ///
        ///     POST /api/company/{id}
        ///     {
        ///       "description": "An edited description here. /n with newlines?"
        ///      }
        /// </remarks>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="queue"></param>
        /// <param name="user"></param>
        /// <response code="200">Is returned when a valid company object is supplied and is successfully modified.</response>
        /// <response code="400">Is returned when a non valid company object is supplied.</response>
        /// <response code="404">Is returned when the company isn't found. </response>
        [FunctionName("CompanyPut")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompanyPut([HttpTrigger(AuthorizationLevel.User, "put", Route = "company/{id:int}")]
                                                    [RequestBodyType(typeof(CompanyPutVM), "The company to be edited")] HttpRequest req, int id,
                                                    [Queue("CompanyPutQueue")] IAsyncCollector<CompanyPutVM> queue,
                                                    [SwaggerIgnore] ClaimsPrincipal user)
        {
            Logger.LogInformation("C# [HTTP] Company endpoint has been triggered with a PUT request.");
            var identity = (ClaimsIdentity)user.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                CompanyPutVM companyPutVM = JsonSerializer.Deserialize<CompanyPutVM>(requestBody, _JsonSerializerOptions);

                if (companyPutVM.Id == null)
                {
                    companyPutVM.Id = id;
                }
                else if (companyPutVM.Id != id)
                {
                    return new BadRequestObjectResult($"Path parameter: {id} is not the same as supplied Id in object: {companyPutVM.Id}. Emit Id from given object.") { StatusCode = StatusCodes.Status400BadRequest };
                }

                try
                {
                    if (await _companyService.CheckIfUserIsAuthorized(identity, (int)companyPutVM.Id))
                    {
                        if (companyPutVM.Name != null)
                        {
                            if (!await _companyService.CheckIfCompanynameExistsAsync(companyPutVM.Name))
                            {
                                await queue.AddAsync(companyPutVM);
                                return new OkObjectResult("Modifications to company where succesfully added to queue and will be processed.") { StatusCode = StatusCodes.Status200OK };
                            }
                            return new ConflictObjectResult("Given company name not permitted. Please try with a different name.") { StatusCode = StatusCodes.Status409Conflict };
                        }
                        await queue.AddAsync(companyPutVM);
                        return new OkObjectResult("Modifications to company where succesfully added to queue and will be processed.") { StatusCode = StatusCodes.Status200OK };
                    }
                    throw new UnauthorizedAccessException();
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.LogError(e.Message);
                    return new UnauthorizedObjectResult(e.Message) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            catch (JsonException e)
            {
                Logger.LogError(e.Message);

                return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }

        /// <summary>
		/// This is the function that's responsible for handling the messages that are added to the Company Put Queue.
		/// </summary>
		/// <param name="message"></param>
		[FunctionName("CompanyPutQueueTrigger")]
        public async Task CompanyPutQueueTriggerAsync([QueueTrigger("CompanyPutQueue")] string message)
        {
            try
            {
                CompanyPutVM companyPutVM = JsonSerializer.Deserialize<CompanyPutVM>(message, _JsonSerializerOptions);

                await _companyService.EditCompanyAsync(companyPutVM);
                Logger.LogInformation($"The following university has been modified: {companyPutVM.Name}");
            }
            catch (JsonException e)
            {
                Logger.LogInformation($"error occured: {e}");
            }
        }

        /// <summary>
        /// This method allows the admin of a company to add more companyusers to the company
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [FunctionName("AddCompanyUserToCompany")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCompanyUserToCompany([HttpTrigger(AuthorizationLevel.User, "post", Route = "company/{id:int}/addcompanyuser")]
                                                                 [RequestBodyType(typeof(CompanyAddUserVM), "The user to be added")] HttpRequest req, int id,
                                                                 [SwaggerIgnore] ClaimsPrincipal user
            )
        {

            Logger.LogInformation("C# [HTTP] Company endpoint AddCompanyUserToCompany has been triggered");

            var identity = (ClaimsIdentity)user.Identity;
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                CompanyAddUserVM userToAdd = JsonSerializer.Deserialize<CompanyAddUserVM>(requestBody, _JsonSerializerOptions);

                if (await _companyService.AddCompanyUserToCompany(userToAdd, identity, id))
                {
                    return new OkObjectResult("User added to company") { StatusCode = StatusCodes.Status200OK };
                }

                return new BadRequestObjectResult("Something went wrong. Check your input and try again") { StatusCode = StatusCodes.Status400BadRequest };
            }
            catch (Exception e)
            {
                Logger.LogInformation($"error occured: {e}");

                return new BadRequestObjectResult(e.Message) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }
    }
}
