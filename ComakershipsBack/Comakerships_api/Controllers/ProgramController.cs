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
    /// class definition for the ProgramController. It contains endpoints related to Programs.
    /// </summary>
    class ProgramController
    {
        private readonly ILogger _logger;
        private readonly IProgramService _programService;
        private readonly JsonSerializerOptions _options;

        /// <summary>
        /// Constructor for the ProgramController
        /// </summary>
        public ProgramController(ILogger<ProgramController> logger, IProgramService programService)
        {
            _logger = logger;
            _programService = programService;            
            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true                
            }.SetupExtensions();
        }

        /// <summary>
        /// This method returns a collections of all programs
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("GetAllPrograms")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPrograms([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "programs")] HttpRequest req)
        {
            _logger.LogInformation("Getting all programs.");

            var programs = await _programService.GetPrograms();
            return new OkObjectResult(programs);
        }

        /// <summary>
        /// This method returns a specific program entity from the provided id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id">Integer</param>
        /// <returns></returns>
        [FunctionName("GetProgram")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProgram([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "programs/{id:int}")] HttpRequest req, int id)
        {
            _logger.LogInformation("Getting program: " + id);

            var program = await _programService.GetProgram(id);
            if (program != null)
            {
                return new OkObjectResult(program);
            }
            return new NotFoundResult();
        }        
    }
}
