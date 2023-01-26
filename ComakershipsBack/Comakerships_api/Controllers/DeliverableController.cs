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
using System.Net.Http;
using System.IO;
using Comakerships_Api.Filters;
using Comakerships_Api.Security;
using System.Collections.Generic;
using Models.ViewModels;
using System.Security.Claims;
using System.Linq;



namespace ComakershipsApi
{
    /// <summary>
    /// class definition for the DeliverableController. It contains endpoints related to Deliverables.
    /// </summary>
    public class DeliverableController{

        private readonly ILogger _logger;
        private readonly IDeliverableService _deliverableService;
        private readonly IComakershipService _comakershipService;
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Constructor for the DeliverableController
        /// </summary>
        public DeliverableController(ILogger<DeliverableController> logger, IDeliverableService deliverableService, IComakershipService comakershipService)
        {
            _logger = logger;
            _deliverableService = deliverableService;            
            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }.SetupExtensions();
            _comakershipService = comakershipService;
        }

        /// <summary>
        /// The method allows you to retreive all deliverables for a comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="comakershipId">Integer</param>
        /// <returns></returns>
        [FunctionName("GetDeliverables")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeliverables([HttpTrigger(AuthorizationLevel.User, "get", Route = "comakerships/{comakershipId:int}/deliverables")] HttpRequest req, int comakershipId) {
            _logger.LogInformation("Getting all deliverables.");

            var deliverables = await _deliverableService.GetDeliverables(comakershipId);          
            return new OkObjectResult(deliverables);
        }

        /// <summary>
        /// This method allows you to retreive a specific deliverable, by supplying the get method with an id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("GetDeliverable")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeliverable([HttpTrigger(AuthorizationLevel.User, "get", Route = "deliverables/{id:int}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Getting deliverable: " + id);

            var deliverable = await _deliverableService.GetDeliverable(id);
            if (deliverable != null)
            {
                return new OkObjectResult(deliverable);
            }
            return new NotFoundResult();                
        }

        /// <summary>
        /// The method allows you to add new deliverables by doing a post request
        /// </summary>
        /// <param name="req"></param>
        /// <param name="comakershipId">Integer</param>
        /// <returns></returns>
        [FunctionName("CreateDeliverable")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDeliverable([HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships/{comakershipId:int}/deliverables")]
                                                   [RequestBodyType(typeof(DeliverablePost), "The deliverable to store")] HttpRequest req, int comakershipId)
        {
            _logger.LogInformation("Adding deliverable.");

            try
            {
                var postData = await JsonSerializer.DeserializeAsync<DeliverablePost>(req.Body, _options);
                try
                {
                    int id = await _deliverableService.CreateDeliverable(comakershipId, postData);
                    return new OkObjectResult($"Deliverable with id '{id}' added");
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e.Message);
                }                               
            }
            catch (JsonException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }


        /// <summary>
        /// This method allows you to edit a deliverable entity
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("EditDeliverable")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> EditDeliverable([HttpTrigger(AuthorizationLevel.User, "put", Route = "deliverables/{id:int}")]
                                                   [RequestBodyType(typeof(DeliverablePut), "The deliverable to edit")] HttpRequest req, int id)
        {
            _logger.LogInformation("Updating deliverable.");

            try
            {
                var putData = await JsonSerializer.DeserializeAsync<DeliverablePut>(req.Body, _options);
                if (putData.Id == id)
                {
                    if (await _deliverableService.UpdateDeliverable(putData))
                    {
                        return new AcceptedResult();
                    }
                    else return new UnprocessableEntityObjectResult("Failed to update");
                }
                else return new BadRequestObjectResult("Id's of data and endpoint don't match");
            }
            catch (JsonException e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /// <summary>
        /// This method allows you to retreive a specific deliverable, by supplying the get method with an id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("DeleteDeliverable")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDeliverable([HttpTrigger(AuthorizationLevel.User, "delete", Route = "deliverables/{id:int}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Endpoint 'delete deliverable' is reached.");

            if (await _deliverableService.DeleteDeliverable(id))
            {
                return new OkObjectResult("Deliverable was successfully deleted");
            }
            return new BadRequestObjectResult("Failed to delete deliverable");

        }

        /// <summary>
        /// This method allows you to upload a file, by providing a multipart formdata
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("UploadDeliverable")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [FileUploadOperation.FileContentType]
        public async Task<IActionResult> UploadDeliverable([HttpTrigger(AuthorizationLevel.User, "post", Route = "comakerships/{id}/file")]
                                                     HttpRequestMessage req, int id)
        {
            if (await _deliverableService.checkIfComakershipExistAsync(id))
            {
                var blobName = "";
                if (req.Content.IsMimeMultipartContent())
                {
                    var provider = new RestrictiveMultipartMemoryStreamProvider();
                    await req.Content.ReadAsMultipartAsync(provider);

                    foreach (HttpContent ctnt in provider.Contents)
                    {
                        var stream = await ctnt.ReadAsStreamAsync();

                        if (stream.Length == 0)
                        {
                            return new BadRequestObjectResult($"Something went wrong. Please try again.") { StatusCode = StatusCodes.Status400BadRequest };
                        }

                        // retrieving filename and content from content disposition. Risky
                        var fileName = ctnt.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                        // generating blobname based on guid+ext
                        blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

                        var ms = new MemoryStream();

                        try
                        {
                            // resetting stream to start from beginning
                            stream.CopyTo(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            await ms.FlushAsync();

                            await _deliverableService.UploadDeliverableToAzureStorageBlob(id, blobName, ms, ctnt);
                            return new OkObjectResult($"{blobName} uploaded") { StatusCode = StatusCodes.Status200OK };
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
            }
            return new BadRequestObjectResult($"Something went wrong. Please try again.") { StatusCode = StatusCodes.Status400BadRequest };
        }

        /// <summary>
        /// This method allows you to retrieve a list of all uploaded files of a comakership
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [FunctionName("GetListComakershipFiles")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetListComakershipFiles([HttpTrigger(AuthorizationLevel.User, "get", Route = "comakerships/{id}/files")]
                                                     HttpRequestMessage req, [SwaggerIgnore] ClaimsPrincipal User, int id)
        {
            // removed check if comakership exists, is redundant and the different errors could give hackers an indication of what comakerships exist
            IEnumerable<ComakershipComplete> userComakerships = await _comakershipService.GetComakershipsForUser((ClaimsIdentity)User.Identity);
            if (userComakerships.FirstOrDefault(c => c.Id == id) == null)
            {
                return new BadRequestObjectResult("Comakership does not exist or you do not participate in it") { StatusCode = StatusCodes.Status403Forbidden };
            }
            try
            {
                List<FileVM> files = await _deliverableService.GetListComakershipFilesFromAzureStorageBlob(id);
                return new OkObjectResult(files) { StatusCode = StatusCodes.Status200OK };
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
