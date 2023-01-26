using Models;
using Moq;
using NUnit.Framework;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComakershipsApi.Controllers;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ComakershipsApi.Infrastructure;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Security.Claims;
using System.Collections.ObjectModel;

namespace NUnitTestControllers
{
    public class UnitTestComakershipController
    {
        private List<ComakershipBasic> _MockComakershipsList;
        private List<ComakershipComplete> _MockComakershipsListFull;
        private List<ComakershipComplete> _MockComakershipsCompleteList;
        private Mock<IComakershipService> _MockComakershipService;
        private Mock<IKeyService> _MockKeyService;
        private Mock<IProgramService> _MockProgramService;
        private ILogger<ComakershipController> _MockLogger;
        private IMapper _MockMapper;

        [SetUp]
        public void Setup()
        {
            _MockComakershipService = new Mock<IComakershipService>();
            _MockKeyService = new Mock<IKeyService>();
            _MockProgramService = new Mock<IProgramService>();

            //Comakership ComakershipOne = new Comakership { Id = 1, Name = "Create an Enrichment Centre", Description = "We are in need of a new Aperture Science Enrichment Center", ComakershipStatusId = 1, Credits = false, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };
            //Comakership ComakershipTwo = new Comakership { Id = 2, Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", ComakershipStatusId = 1, Credits = true, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };
            //_MockComakershipsList = new List<Comakership> { ComakershipOne, ComakershipTwo };

            ComakershipBasic Comakership1 = new ComakershipBasic { Id = 1, Name = "Create an Enrichment Centre", Description = "We are in need of a new Aperture Science Enrichment Center", Credits = false, Bonus = true, Skills = null, CreatedAt = DateTime.Parse("10-10-2020") };
            ComakershipBasic Comakership2 = new ComakershipBasic { Id = 2, Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", Credits = true, Bonus = true, Skills = null, CreatedAt = DateTime.Parse("10-10-2020") };
            _MockComakershipsList = new List<ComakershipBasic> { Comakership1, Comakership2 };

            ComakershipComplete Comakership1Full = new ComakershipComplete { Id = 1, Name = "Create an Enrichment Centre", Description = "We are in need of a new Aperture Science Enrichment Center", Credits = false, Bonus = true, Skills = null, CreatedAt = DateTime.Parse("10-10-2020") };
            ComakershipComplete Comakership2Full = new ComakershipComplete { Id = 2, Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", Credits = false, Bonus = true, Skills = null, CreatedAt = DateTime.Parse("10-10-2020") };
            _MockComakershipsListFull = new List<ComakershipComplete> { Comakership1Full, Comakership2Full };

            ComakershipComplete ComakershipComplete1 = new ComakershipComplete { Id = 1, Name = "Dummy comaker", Description = "Just some text", Bonus = false, Credits = true, Skills = null, Deliverables = null, CreatedAt = DateTime.Parse("10-10-2020") };
            _MockComakershipsCompleteList = new List<ComakershipComplete> { ComakershipComplete1 };

            _MockComakershipService.Setup(service => service.GetComakerships()).ReturnsAsync(_MockComakershipsList);
            _MockComakershipService.Setup(service => service.GetComakership(It.Is<int>(i => i == 1))).ReturnsAsync(_MockComakershipsList.FirstOrDefault(c => c.Id == 1));
            _MockComakershipService.Setup(service => service.GetComakershipComplete(It.Is<int>(i => i == 1))).ReturnsAsync(_MockComakershipsCompleteList.FirstOrDefault(c => c.Id == 1));
            _MockComakershipService.Setup(service => service.UpdateComakership(It.Is<ComakershipPut>(x => x.Id == 1), It.IsAny<ClaimsIdentity>())).Returns(Task.FromResult(true));
            _MockComakershipService.Setup(service => service.AcceptApplication(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ClaimsIdentity>())).Returns(Task.FromResult(true));
            _MockComakershipService.Setup(service => service.GetComakershipsForUser(It.IsAny<ClaimsIdentity>())).Returns(Task.FromResult(_MockComakershipsListFull.AsEnumerable()));

            _MockKeyService.Setup(service => service.ValidateKey(It.IsAny<string>())).ReturnsAsync(true);

            ProgramGet program = new ProgramGet { Id = 1, Name = "IT" };
            _MockProgramService.Setup(service => service.GetProgram(It.IsAny<int>())).ReturnsAsync(program);
            

            _MockLogger = Mock.Of<ILogger<ComakershipController>>();

            // setup mapper
            _MockMapper = Mock.Of<IMapper>();

            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            _MockMapper = mapperConfig.CreateMapper();
        }

        [Test]
        public async Task GetAllComakerships_WhenCalled_Should_Return_TypeOf_ComakershipBasic_Async()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);

            // Act           
            var response = await ComakershipController.GetAllComakerships(httpRequest);
            var okResponse = response as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
            Assert.AreEqual(200, okResponse.StatusCode);
            Assert.IsInstanceOf<IEnumerable<ComakershipBasic>>(okResponse.Value);
        }


        [Test]
        public async Task GetComakership_WhenCalled_Should_Return_TypeOf_ComakershipBasic_Async()
        {
            // Arrange
            var httpRequest = Mock.Of<HttpRequest>();
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);

            // Act
            var response = await ComakershipController.GetComakership(httpRequest, 1);
            var okResponse = response as ObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
            Assert.AreEqual(200, okResponse.StatusCode);
            Assert.IsInstanceOf<ComakershipBasic>(okResponse.Value);
        }

        //GetComakershipComplete
        [Test]
        public async Task GetComakershipComplete_WhenCalled_Should_Return_TypeOf_ComakershipComplete()
        {
            //Arrange
            var httpRequest = Mock.Of<HttpRequest>();
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);

            //Act
            var response = await ComakershipController.GetComakershipComplete(httpRequest, 1);
            var okResponse = response as ObjectResult;

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
            Assert.AreEqual(200, okResponse.StatusCode);
            Assert.IsInstanceOf<ComakershipComplete>(okResponse.Value);
        }

        //AddComakership
        [Test]
        public async Task AddComakership_WhenCalled_Should_Return_OkObjectResult_And_Code_Async()
        {
            // Arrange            
            Collection<int> postedPrograms = new Collection<int> { 1,2,3};
            ComakershipPost newComakership = new ComakershipPost { Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", Credits = true, Bonus = true, ProgramIds = postedPrograms, PurchaseKey = "ABC123" };
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);
            Mock<HttpRequest> mockRequest = CreateMockRequest(newComakership);
            var claims = new List<Claim>()
            {
                new Claim("UserType", "CompanyUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            var response = await ComakershipController.AddComakership(mockRequest.Object, claimsPrincipal);
            var okResponse = response as OkObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(okResponse);
            Assert.True(okResponse is OkObjectResult);
            Assert.AreEqual(201, okResponse.StatusCode);
            Assert.AreEqual("Comakership with id '0' added", okResponse.Value.ToString());
        }

        [Test]
        public async Task AddComakership_WhenCalled_With_Empty_ComakershipObject_Should_Return_BadRequestResponse_And_Code_Async()
        {
            // Arrange
            Comakership newComakership = new Comakership { };
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);
            Mock<HttpRequest> mockRequest = CreateMockRequest(newComakership);
            var claims = new List<Claim>()
            {
                new Claim("UserType", "CompanyUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);


            // Act
            var response = await ComakershipController.AddComakership(mockRequest.Object, claimsPrincipal);
            var okResponse = response as BadRequestObjectResult;

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(okResponse);
            Assert.True(okResponse is BadRequestObjectResult);
            Assert.AreEqual(400, okResponse.StatusCode);
            Assert.AreEqual("Property 'Name' cannot be null.", okResponse.Value.ToString());
        }

        //AddComakership
        [Test]
        public async Task UpdateComakership_WhenCalled_Should_Return_AcceptedResponse_And_Code_Async()
        {
            // Arrange
            Comakership updateComakership = new Comakership { Id = 1, Name = "Edited name", Description = "edited description", Credits = true, Bonus = true };
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);
            Mock<HttpRequest> mockRequest = CreateMockRequest(updateComakership);
            var claims = new List<Claim>()
            {
                new Claim("UserType", "CompanyUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            var response = await ComakershipController.EditComakership(mockRequest.Object, 1, claimsPrincipal);
            var okResponse = response as AcceptedResult;

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(okResponse);
            Assert.True(okResponse is AcceptedResult);
            Assert.AreEqual(202, okResponse.StatusCode);
        }

        [Test]
        public async Task AcceptApplication_WhenCalled_Should_Return_OkResult() {
            var claims = new List<Claim>()
            {
                new Claim("UserType", "CompanyUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);

            ObjectResult result = (ObjectResult)await ComakershipController.AcceptApplication(null, claimsPrincipal, 1, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetComakershipsForUser_Should_Return_List() {

            Mock<ClaimsPrincipal> claims = new Mock<ClaimsPrincipal>();

            ComakershipController ComakershipController = new ComakershipController(_MockLogger, _MockComakershipService.Object);

            ObjectResult result = (ObjectResult)await ComakershipController.GetUserComakerships(null, claims.Object);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
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
        public void TestCleanUp()
        {
            _MockComakershipService = null;
        }
    }
}
