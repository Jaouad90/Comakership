using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dahomey.Json;
using System.Text.Json;
using Models;
using AutoMapper;
using ServiceLayer;
using System;

namespace ComakershipsApi.Controllers
{
    /// <summary>
    /// class definition for the ComakershipStatusController. It contains endpoints related to ComakerhipStatus.
    /// </summary>
    class ComakershipStatusController
    {
        private readonly ILogger _logger;
        private readonly IStatusService _statusService;
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Constructor for the ComakershipStatusController
        /// </summary>
        public ComakershipStatusController(ILogger<ComakershipController> logger, IStatusService statusService)
        {
            _logger = logger;
            _statusService = statusService;            
            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true                
            }.SetupExtensions();
        }

        /// <summary>
        /// This method returns a collections of all statuses
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetAllStatuses")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStatuses([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "statuses")] HttpRequest req)
        {
            _logger.LogInformation("Getting all statuses.");

            var statuses = await _statusService.GetStatuses();
            return new OkObjectResult(statuses);
        }        
    }
}
