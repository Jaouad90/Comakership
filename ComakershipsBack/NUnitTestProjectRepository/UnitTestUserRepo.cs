using DAL;
using Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTestProjectRepository {
    public class UnitTestUserRepo {

        public List<UserBody> _mockListUsers;

        public University _mockUniversity;

        public Company _mockCompany;

        public Mock<IUserRepository> dalMock;

        public UnitTestUserRepo() {
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

            _mockListUsers = new List<UserBody>() {
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
                    Password = "uhrfuiwehrfuiwhriurhewruihwer8974238yqwjs89j8",
                    Deleted = false,
                    Company = _mockCompany,
                }
            };

            dalMock = new Mock<IUserRepository>();
            
            //Get Student
            dalMock.Setup(d => d.GetUser<UserBody>(It.IsAny<int>())).ReturnsAsync((int s) => _mockListUsers.FirstOrDefault(u => u.Id == s));

        }

        [Test]
        public async Task GetStudentUserShouldReturnUser() {
            StudentUser user = (StudentUser) await dalMock.Object.GetUser<UserBody>(1);

            Assert.True(user.University == _mockUniversity);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public async Task GetCompanyUserShouldReturnUser() {
            CompanyUser user = (CompanyUser) await dalMock.Object.GetUser<UserBody>(2);

            Assert.True(user.Company == _mockCompany);
            Assert.AreEqual(2, user.Id);
        }
    }
}
