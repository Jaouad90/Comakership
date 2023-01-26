using AutoMapper;
using Castle.Core.Logging;
using ComakershipsApi;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ServiceLayer.User;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Models.ViewModels;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ComakershipsApi.Infrastructure;
using System.Security.Claims;
using ServiceLayer;
using System.Linq.Expressions;

namespace NUnitTestControllers {
    class UnitTestUserController {

        private List<UserBody> _mockUserList;
        private Mock<IUserService> _mockUserService;
        private Mock<ITeamService> _mockTeamService;
        private University _mockUniversity;
        private Company _mockCompany;
        private Logger<UserController> _logger;
        private IMapper _mapper;
        private UserController _controller;

        [SetUp]
        public void Setup() {
            _logger = new Logger<UserController>(new LoggerFactory());

            _mockUserService = new Mock<IUserService>();

            _mockTeamService = new Mock<ITeamService>();

            _mockUniversity = new University() {
                Id = 1,
                Name = "Inholland",
                RegistrationDate = new DateTime(2020, 01, 01),
                Domain = "Inholland.nl",
                Street = "SesamStraat 123",
                City = "Haarlem",
                Zipcode = "2015CL"
            };

            _mockCompany = new Company() {
                Id = 1,
                Name = "CoolBedrijf",
                Description = "Super innovatief en cool bedrijf enzo",
                RegistrationDate = new DateTime(2020, 01, 01),
                Street = "SesamStraat 123",
                City = "Haarlem",
                Zipcode = "2015CL"
            };

            _mockUserList = new List<UserBody>() {
                new StudentUser() {
                    Id = 1,
                    Name = "Henk",
                    Email = "henk@student.inholland.com",
                    Password = "10000.e91IjAutQy9+ViXfueYZ7g==.NOvnd8Wmlne+aqVZD+jpMBdrbAXN+RJcwtfrxUbB9Gs=",
                    Deleted = false,
                    StudentNumber = 123456,
                    About = "I am a very good student and always willing to work for free hire me now.",
                    University = _mockUniversity,
                    PrivateTeamId = 1
                },
                new CompanyUser() {
                    Id = 2,
                    Name = "Piet",
                    Email = "piet@coolbedrijf.nl",
                    Password = "10000.e91IjAutQy9+ViXfueYZ7g==.NOvnd8Wmlne+aqVZD+jpMBdrbAXN+RJcwtfrxUbB9Gs=",
                    Deleted = false,
                    Company = _mockCompany,
                }
            };

            _mockUserService.Setup(s => s.GetUser<UserBody>(It.IsAny<int>())).ReturnsAsync((int i) => _mockUserList.FirstOrDefault(l => l.Id == i));

            _mockUserService.Setup(s => s.GetSingle(It.IsAny< Expression<Func<UserBody,bool>>> ())).ReturnsAsync(_mockUserList.FirstOrDefault());

            _mockUserService.Setup(d => d.SaveUser<UserBody>(It.IsAny<UserBody>())).Callback(new Action<UserBody>(
                        x => {
                            _mockUserList.Add(x);
                        }
                    )).ReturnsAsync(true);

            _mockUserService.Setup(d => d.EditUser<UserBody>(It.IsAny<UserBody>(), It.IsAny<int>())).ReturnsAsync(true);

            _mockUserService.Setup(d => d.DeleteUser<UserBody>(It.IsAny<int>(), It.IsAny<ClaimsIdentity>())).Callback(new Action<int, ClaimsIdentity>((x, y) => {
                var found = _mockUserList.Find(c => c.Id == x);
                var index = _mockUserList.IndexOf(found);
                _mockUserList[index].Deleted = true;
            })).ReturnsAsync(true);

            _mockTeamService.Setup(s => s.CreateTeam(It.IsAny<TeamPost>(), It.IsAny<int>())).ReturnsAsync((TeamPost t, int i) => 2);

            _mockUserService.Setup(d => d.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            _mockUserService.Setup(d => d.AddSkill(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            _mockUserService.Setup(d => d.DeleteSkill(It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            _mockUserService.Setup(d => d.EditSkill(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            _mockUserService.Setup(d => d.EditStudentUser(It.IsAny<StudentPutVM>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            _mockUserService.Setup(d => d.EditCompanyUser(It.IsAny<CompanyUserPutVM>(), It.IsAny<ClaimsIdentity>())).ReturnsAsync(true);

            _mapper = Mock.Of<IMapper>();
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _controller = new UserController(_logger, _mockUserService.Object, _mapper, _mockTeamService.Object);
        }

        [Test]
        public async Task GetStudentShouldReturnStudent() {
            JsonResult result = (JsonResult) await _controller.StudentGet(null, 1);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value, _mockUserList.FirstOrDefault(u => u.Id == 1));
        }

        [Test]
        public async Task GetCompanyUserShouldReturnCompanyUser() {
            JsonResult result = (JsonResult)await _controller.CompanyUserGet(null, 2);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(result.Value, _mockUserList.FirstOrDefault(u => u.Id == 2));
        }

        [Test]
        public async Task PostStudentShouldReturn201AndAddToDB() {
            StudentPostVM newUser = new StudentPostVM() {
                FirstName = "joey",
                LastName = "m",
                Email = "joey@inholland.nl",
                Password = "test123",
                ProgramId = 1,
                Nickname = "xXCoolestudentXx"
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(newUser);

            ObjectResult result = (ObjectResult)await _controller.StudentPost(mockRequest.Object);

            var userInList = _mockUserList.FirstOrDefault(u => u.Name == "joey m");

            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(userInList);
            Assert.AreEqual(typeof(StudentUser), userInList.GetType());
        }

        [Test]
        public async Task PostCompanyUserShouldReturn201AndAddToDB() {
            CompanyUserPostVM newUser = new CompanyUserPostVM() {
                Name = "sinterklaas",
                Email = "sinterklaas@inholland.nl",
                Password = "test123"
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(newUser);

            ObjectResult result = (ObjectResult)await _controller.CompanyUserPost(mockRequest.Object);

            var userInList = _mockUserList.FirstOrDefault(u => u.Name == "sinterklaas");

            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.IsNotNull(userInList);
            Assert.AreEqual(typeof(CompanyUser), userInList.GetType());
        }

        [Test]
        public async Task PutStudentShouldUpdateUserAndReturn200() {
            var userToEdit = new StudentPutVM() {
                Name = "Henk",
                About = "this is a different description",
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(userToEdit);

            //ClaimsPrincipal claims = Mock.Of<ClaimsPrincipal>();
            var claims = new List<Claim>()
            {
                new Claim("UserId", "1")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            ObjectResult result = (ObjectResult)await _controller.StudentPut(mockRequest.Object, claimsPrincipal);

            var userInList = (StudentUser) _mockUserList.FirstOrDefault(u => u.Id == 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(userInList);
        }

        [Test]
        public async Task PutCompanyUserShouldUpdateUserAndReturn200() {
            var userToEdit = new CompanyUserPutVM() {
                Name = "Pietje"
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(userToEdit);

            //ClaimsPrincipal claims = Mock.Of<ClaimsPrincipal>();

            var claims = new List<Claim>()
            {
                new Claim("UserId", "2")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            ObjectResult result = (ObjectResult)await _controller.CompanyUserPut(mockRequest.Object, claimsPrincipal);

            var userInList = (CompanyUser)_mockUserList.FirstOrDefault(u => u.Id == 2);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(userInList);
        }

        [Test]
        public async Task DeleteStudentShouldDeleteUserAndReturn200() {
            ClaimsPrincipal claims = Mock.Of<ClaimsPrincipal>();
            ObjectResult result = (ObjectResult) await _controller.StudentDelete(null, claims, 1);

            var userInList = _mockUserList.FirstOrDefault(u => u.Id == 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(userInList.Deleted);
        }

        [Test]
        public async Task DeleteCompanyUserShouldDeleteUserAndReturn200() {
            ClaimsPrincipal claims = Mock.Of<ClaimsPrincipal>();
            ObjectResult result = (ObjectResult)await _controller.CompanyUserDelete(null, claims, 2);

            var userInList = _mockUserList.FirstOrDefault(u => u.Id == 2);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsTrue(userInList.Deleted);
        }

        [Test]
        public async Task ChangePasswordShouldReturn200() {
            var changePasswordObject = new ChangePasswordVM() {
                OldPassword = "test123",
                NewPassword = "test1234",
                ConfirmNewPassword = "test1234"
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(changePasswordObject);

            ClaimsPrincipal claims = Mock.Of<ClaimsPrincipal>();
            ObjectResult result = (ObjectResult)await _controller.ChangePassword(mockRequest.Object, claims);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task AddSkillShouldReturn200() {
            string skill = "C#";

            Mock<HttpRequest> mockRequest = CreateMockRequest(skill);

            var claims = new List<Claim>()
            {
                new Claim("UserType", "StudentUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            ObjectResult result = (ObjectResult)await _controller.AddSkill(mockRequest.Object, claimsPrincipal);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task DeleteSkillShouldReturn200() {
            string skill = "C#";

            Mock<HttpRequest> mockRequest = CreateMockRequest(skill);

            var claims = new List<Claim>()
            {
                new Claim("UserType", "StudentUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            ObjectResult result = (ObjectResult)await _controller.DeleteSkill(mockRequest.Object, claimsPrincipal);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task EditSkillShouldReturn200() {
            var editSkillVm = new EditSkillVM() {
                OldSkill = "C#",
                NewSkill = "Java"
            };

            Mock<HttpRequest> mockRequest = CreateMockRequest(editSkillVm);

            var claims = new List<Claim>()
            {
                new Claim("UserType", "StudentUser")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            ObjectResult result = (ObjectResult)await _controller.EditSkill(mockRequest.Object, claimsPrincipal);

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        private static Mock<HttpRequest> CreateMockRequest(object body) {
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
    }
}