using AutoMapper;
using ComakershipsApi.Infrastructure;
using DAL;
using Models;
using Models.ViewModels;
using Moq;
using NUnit.Framework;
using ServiceLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTestProjectServiceLayer {
    class UnitTestUserServiceLayer {
        private List<UserBody> _mockUserList;
        private Mock<IUserRepository> _mockUserRepo;
        private University _mockUniversity;
        private Company _mockCompany;
        private UserService _userService;
        private Mock<IUniversityRepository> _mockUniversityRepo;
        private IMapper _mockMapper;

        [SetUp]
        public void SetUp() {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockUniversityRepo = new Mock<IUniversityRepository>();

            _mockMapper = Mock.Of<IMapper>();
            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            _mockMapper = mapperConfig.CreateMapper();

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
                    PrivateTeamId = 1,
                    Skills = {
                        "Java"
                    }
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

            _mockUserRepo.Setup(d => d.GetUser<UserBody>(It.IsAny<int>())).ReturnsAsync((int i) => _mockUserList.FirstOrDefault(l => l.Id == i));

            _mockUserRepo.Setup(d => d.Add(It.IsAny<UserBody>())).Callback(new Action<UserBody>(
                        x => {
                            _mockUserList.Add(x);
                        }
                    )).ReturnsAsync(true);

            _mockUserRepo.Setup(d => d.Update(It.IsAny<UserBody>())).Callback(new Action<UserBody>(
                        x => {
                            var found = _mockUserList.Find(c => c.Id == x.Id);
                            var index = _mockUserList.IndexOf(found);
                            _mockUserList[index] = x;
                        }
                    )).ReturnsAsync(true);

            _mockUserRepo.Setup(d => d.DeleteWhere(It.IsAny<Expression<Func<UserBody, bool>>>())).ReturnsAsync(true);

            _mockUserRepo.Setup(d => d.GetSingle(It.IsAny<Expression<Func<UserBody, bool>>>())).ReturnsAsync(_mockUserList.FirstOrDefault());


            University u1 = new University {
                Id = 66,
                Name = "Inholland",
                Street = "Bijdorplaan 15",
                City = "Haarlem",
                Zipcode = "2015CE",
                RegistrationDate = new DateTime(637328304000000000),
                Domain = "@student.inholland.nl"
            };

            _mockUniversityRepo.Setup(d => d.GetByDomain(It.IsAny<string>())).ReturnsAsync(u1);

            _userService = new UserService(_mockUserRepo.Object, _mockUniversityRepo.Object, _mockMapper);
        }

        [Test]
        public async Task GetStudentShouldReturnStudent() {
            var user = await _userService.GetUser<StudentUser>(1);

            Assert.IsNotNull(user);
            Assert.AreEqual(typeof(StudentUser), user.GetType());
            Assert.AreEqual("Henk", user.Name);
        }

        [Test]
        public async Task GetCompanyUserShouldReturnCompanyUser() {
            var user = await _userService.GetUser<CompanyUser>(2);

            Assert.IsNotNull(user);
            Assert.AreEqual(typeof(CompanyUser), user.GetType());
            Assert.AreEqual("Piet", user.Name);
        }

        [Test]
        public async Task SaveStudentShouldReturnTrue() {
            var studentToAdd = new StudentUser() {
                Id = 3,
                Name = "Klaas",
                Email = "klaas@student.inholland.com",
                Password = "10000.e91IjAutQy9+ViXfueYZ7g==.NOvnd8Wmlne+aqVZD+jpMBdrbAXN+RJcwtfrxUbB9Gs=",
                Deleted = false,
                StudentNumber = 987654,
                About = "I am klaas hello there.",
                University = _mockUniversity
            };

            var result = await _userService.SaveUser<StudentUser>(studentToAdd);

            Assert.IsTrue(result);
            Assert.IsNotNull(_mockUserList.FirstOrDefault(u => u.Id == 3));
        }

        [Test]
        public async Task SaveCompanyUserShouldReturnTrue() {
            var companyUserToAdd = new CompanyUser() {
                Id = 4,
                Name = "Truus",
                Email = "truus@coolbedrijf.nl",
                Password = "10000.e91IjAutQy9+ViXfueYZ7g==.NOvnd8Wmlne+aqVZD+jpMBdrbAXN+RJcwtfrxUbB9Gs=",
                Deleted = false,
                Company = _mockCompany,
            };

            var result = await _userService.SaveUser<CompanyUser>(companyUserToAdd);

            Assert.IsTrue(result);
            Assert.IsNotNull(_mockUserList.FirstOrDefault(u => u.Id == 4));
        }

        [Test]
        public async Task EditStudentShouldReturnTrue() {
            var studentToEdit = new StudentUser() {
                Id = 1,
                Name = "Henkie",
                Email = "henk@student.inholland.com",
                Password = "10000.e91IjAutQy9+ViXfueYZ7g==.NOvnd8Wmlne+aqVZD+jpMBdrbAXN+RJcwtfrxUbB9Gs=",
                Deleted = false,
                StudentNumber = 123456,
                About = "I have a different description now",
                University = _mockUniversity,
                PrivateTeamId = 1
            };

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));
            int id = int.Parse(claims.Object.FindFirst("UserId").Value);
            // TODO : fix test
            var result = await _userService.EditUser<StudentUser>(studentToEdit, id);

            var editedUser = _mockUserList.FirstOrDefault(u => u.Id == 1) as StudentUser;

            Assert.IsTrue(result);
            Assert.AreEqual("Henkie", editedUser.Name);
            Assert.AreEqual("I have a different description now", editedUser.About);
        }

        [Test]
        public async Task EditUserShouldReturnTrue() {
            var companyUserToEdit = new CompanyUser() {
                Id = 2,
                Name = "Pietje",
                Email = "piet@coolbedrijf.nl",
                Password = "10000.e91IjAutQy9+ViXfueYZ7g==.NOvnd8Wmlne+aqVZD+jpMBdrbAXN+RJcwtfrxUbB9Gs=",
                Deleted = false,
                Company = _mockCompany,
            };

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "2"));
            int id = int.Parse(claims.Object.FindFirst("UserId").Value);

            var result = await _userService.EditUser<CompanyUser>(companyUserToEdit, id);

            var editedUser = _mockUserList.FirstOrDefault(u => u.Id == 2) as CompanyUser;

            Assert.IsTrue(result);
            Assert.AreEqual("Pietje", editedUser.Name);
        }

        [Test]
        public async Task DeleteStudentShouldReturnTrue() {
            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _userService.DeleteUser<StudentUser>(1, claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteCompanyUserShouldReturnTrue() {

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "2"));

            var result = await _userService.DeleteUser<CompanyUser>(2, claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetUserByEmailShouldReturnUser() {
            var user = await _userService.GetSingle(u => u.Email == "henk@student.inholland.com");

            Assert.IsNotNull(user);
            Assert.AreEqual(typeof(StudentUser), user.GetType());
            Assert.AreEqual("Henk", user.Name);
        }

        [Test]
        public async Task IsValidLoginShouldReturnTrue() {
            var result = await _userService.IsValidLogin("henk@student.inholland.com", "test123");

            Assert.IsTrue(result);
        }

        [Test]
        public async Task ChangePasswordShouldReturnTrue() {
            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _userService.ChangePassword("test123", "test1234", "test1234", claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddSkillShouldReturnTrue() {
            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserType")).Returns(new Claim("UserType", "StudentUser"));
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _userService.AddSkill("C#", claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteSkillShouldReturnTrue() {
            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserType")).Returns(new Claim("UserType", "StudentUser"));
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _userService.DeleteSkill("Java", claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditStudentUserShouldReturnTrue() {
            var userToEdit = new StudentPutVM() {
                Name = "Henk",
                About = "this is a different description",
            };

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _userService.EditStudentUser(userToEdit, claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditCompanyUserShouldReturnTrue() {
            var userToEdit = new CompanyUserPutVM() {
                Name = "Pietje"
            };

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "2"));

            var result = await _userService.EditCompanyUser(userToEdit, claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditSkillShouldReturnTrue() {
            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserType")).Returns(new Claim("UserType", "StudentUser"));
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _userService.EditSkill("Java", "C#", claims.Object);

            Assert.IsTrue(result);
        }
    }
}
