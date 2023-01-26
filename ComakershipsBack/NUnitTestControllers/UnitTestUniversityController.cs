using Moq;
using NUnit.Framework;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ComakershipsApi;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Net.Http;
using System.Collections;
using Microsoft.Azure.WebJobs;
using System.Text.Json;
using ComakershipsApi.Infrastructure;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography.X509Certificates;
using Models;
using Models.ViewModels;

namespace NUnitTestControllers
{
    public class UnitTestUniversityController
    {
        private List<University> _universityList;
        private List<UniversityDomainVM> _universityDomainVMList;
        private Mock<IUniversityService> _mockUniversityService;
        private ILogger<UniversityController> _mockLogger;
        private IMapper _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockUniversityService = new Mock<IUniversityService>();

            initializeBogusUniversityList();
            initializeBogusUniversityDomainVMList();

            // Get lists
            _mockUniversityService.Setup(u => u.GetAllUniversitiesAsync()).ReturnsAsync(_universityList);
            _mockUniversityService.Setup(u => u.GetAllUniversityDomainVMAsync()).ReturnsAsync(_universityDomainVMList);

            // get individuals
            _mockUniversityService.Setup(u => u.GetUniversityByIdAsync(It.Is<int>(i => i == 1))).ReturnsAsync(_universityList.FirstOrDefault(u => u.Id == 66));
            _mockUniversityService.Setup(u => u.GetUniversityDomainByIdAsync(It.Is<int>(i => i == 66))).ReturnsAsync(_universityDomainVMList.FirstOrDefault(d => d.Id == 66));

            // Post
            _mockUniversityService.Setup(u => u.SaveUniversityAsync(It.Is<UniversityPostVM>(u => u.Name == "Inholland")));

            // Checking if name exist and only return true when Inholland of HvA is found.
            _mockUniversityService.Setup(u => u.CheckIfUniversitynameExistsAsync(It.IsIn<string>("Inholland", "HvA Kohnstammhuis"))).Callback(new Action<string>(x =>
            {
                University university = _universityList.First(u => u.Name == x);
            }
            )).ReturnsAsync(true);

            // setup logger
            _mockLogger = Mock.Of<ILogger<UniversityController>>();

            // setup mapper
            _mockMapper = Mock.Of<IMapper>();

            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            _mockMapper = mapperConfig.CreateMapper();
        }

        private void initializeBogusUniversityList()
        {
            _universityList = new List<University>();

            University u1 = new University
            {
                Id = 66,
                Name = "Inholland",
                Street = "Bijdorplaan 15",
                City = "Haarlem",
                Zipcode = "2015CE",
                RegistrationDate = new DateTime(637328304000000000),
                Domain = "student.inholland.nl"
            };

            University u2 = new University
            {
                Id = 124,
                Name = "HvA Kohnstammhuis",
                Street = "Wibautstraat 2-4",
                City = "Amsterdam",
                Zipcode = "1091GM",
                RegistrationDate = new DateTime(637328232000000000),
                Domain = "student.hva.nl"
            };

            _universityList.Add(u1);
            _universityList.Add(u2);
        }

        private void initializeBogusUniversityDomainVMList()
        {
            _universityDomainVMList = new List<UniversityDomainVM>();

            UniversityDomainVM u1 = new UniversityDomainVM()
            {
                Id = 66,
                Name = "Hogeschool Inholland",
                Domain = "student.inholland.nl"
            };

            UniversityDomainVM u2 = new UniversityDomainVM()
            {
                Id = 123,
                Name = "Hogeschool van Amsterdam",
                Domain = "student.hva.nl"
            };

            UniversityDomainVM u3 = new UniversityDomainVM()
            {
                Id = 99,
                Name = "Zweinstein",
                Domain = "hocuspocus.zweinstein.co.uk"
            };

            _universityDomainVMList.Add(u1);
            _universityDomainVMList.Add(u2);
            _universityDomainVMList.Add(u3);
        }
        [Test]
        public void AlwaysPass()
        {
            Assert.Pass();
        }

        [Test]
        public async Task Get_Universities_Should_Return_OK()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();

            // act
            UniversityController universityController = new UniversityController(_mockLogger, _mockUniversityService.Object, _mockMapper);
            var response = await universityController.UniversitiesGet(httpRequest);
            var okResponse = response as ObjectResult;

            //Assert
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
            Assert.IsInstanceOf<IEnumerable<University>>(okResponse.Value);
        }

        [Test]
        public async Task Get_UniversityDomains_Should_Return_OK()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();

            // act
            UniversityController universityController = new UniversityController(_mockLogger, _mockUniversityService.Object, _mockMapper);
            var response = await universityController.UniversitiesDomainsGet(httpRequest);
            var okResponse = response as ObjectResult;

            //Assert
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
            Assert.IsInstanceOf<IEnumerable<UniversityDomainVM>>(okResponse.Value);
        }

        [Test]
        public async Task Post_University_Should_Return_OK()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();
            httpRequest.Method = HttpMethod.Post.ToString();
            IAsyncCollector<UniversityPostVM> queue = Mock.Of<IAsyncCollector<UniversityPostVM>>();
            UniversityPostVM universityPostVM = new UniversityPostVM
            {
                Name = "Inholland2",
                City = "Haarlem",
                Street = "Bijdorplaan 15",
                Zipcode = "2015 CE",
                Domain = "test.inholland.nl"
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(universityPostVM);


            // act
            UniversityController universityController = new UniversityController(_mockLogger, _mockUniversityService.Object, _mockMapper);
            var response = await universityController.UniversityPost(mockRequest.Object, queue);
            var okResponse = response as ObjectResult;

            //Assert
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
        }

        [Test]
        public async Task Post_University_Should_Return_BadRequest()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();
            httpRequest.Method = HttpMethod.Post.ToString();
            IAsyncCollector<UniversityPostVM> queue = Mock.Of<IAsyncCollector<UniversityPostVM>>();
            UniversityPostVM universityPostVM = new UniversityPostVM
            {
                City = "Haarlem",
                Street = "Bijdorplaan 15",
                Zipcode = "2015 CE",
                Domain = "test.inholland.nl"
            };
            Mock<HttpRequest> mockRequest = CreateMockRequest(universityPostVM);
            UniversityController universityController = new UniversityController(_mockLogger, _mockUniversityService.Object, _mockMapper);
            // act

            var response = await universityController.UniversityPost(mockRequest.Object, queue);
            var badRequestResponse = response as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResponse);
            Assert.True(badRequestResponse is BadRequestObjectResult);
        }

        [Test]
        public async Task Post_University_Should_Return_Conflict()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();
            httpRequest.Method = "POST";
            IAsyncCollector<UniversityPostVM> queue = Mock.Of<IAsyncCollector<UniversityPostVM>>();
            UniversityPostVM universityPostVM = new UniversityPostVM
            {
                Name = "Inholland",
                City = "Haarlem",
                Street = "Bijdorplaan 15",
                Zipcode = "2015 CE",
                Domain = "test.inholland.nl"
            };
            Mock<HttpRequest> mockRequest = CreateMockRequest(universityPostVM);
            UniversityController universityController = new UniversityController(_mockLogger, _mockUniversityService.Object, _mockMapper);
            
            // act
            var response = await universityController.UniversityPost(mockRequest.Object, queue);
            var conflictResponse = response as ObjectResult;

            //Assert
            Assert.NotNull(conflictResponse);
            Assert.True(conflictResponse is ConflictObjectResult);
        }

        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonSerializer.Serialize(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }

        [TearDown]
        public void TearDown()
        {
            _mockUniversityService = null;
            _mockLogger = null;
            _mockMapper = null;
        }

    }
}