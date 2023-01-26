using AutoMapper;
using ComakershipsApi.Infrastructure;
using DAL;
using Models;
using Moq;
using NUnit.Framework;
using ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NUnitTestProjectServiceLayer
{
    public class UnitTestComakerhipService
    {
        private Mock<IComakershipRepository> _MockComakershipRepository;
        private Mock<IProgramRepository> _MockProgramRepository;
        private Mock<IPurchaseKeyRepository> _MockPurchaseKeyRepository;
        private ComakershipService _ComakershipService;
        private List<Comakership> _MockComakershipsList;
        private IMapper _MockMapper;

        [SetUp]
        public void Setup()
        {
            _MockMapper = Mock.Of<IMapper>();

            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            _MockMapper = mapperConfig.CreateMapper();
            _MockComakershipRepository = new Mock<IComakershipRepository>();
            _MockProgramRepository = new Mock<IProgramRepository>();
            _MockPurchaseKeyRepository = new Mock<IPurchaseKeyRepository>();
            _ComakershipService = new ComakershipService(_MockComakershipRepository.Object, _MockProgramRepository.Object, _MockPurchaseKeyRepository.Object, _MockMapper);

            Comakership ComakershipOne = new Comakership { Id = 1, Name = "Create an Enrichment Centre", Description = "We are in need of a new Aperture Science Enrichment Center", ComakershipStatusId = 1, Credits = false, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };
            Comakership ComakershipTwo = new Comakership { Id = 2, Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", ComakershipStatusId = 1, Credits = true, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };
            _MockComakershipsList = new List<Comakership> { ComakershipOne, ComakershipTwo};


        }

        [Test]
        public void Calling_GetComakerships_ON_ServiceLayer_Should_Call_ComakershipRepo_and_Return_all_MockComakershipsList()
        {
            //Arrange
            _MockComakershipRepository.Setup(rep => rep.GetComakerships()).Returns(Task.FromResult<IEnumerable<Comakership>>(_MockComakershipsList));

            //act
            IEnumerable<ComakershipBasic> result = _ComakershipService.GetComakerships().Result.ToList();

            //Assert
            Assert.AreEqual(result.Count(), 2);
            Assert.That(result, Is.InstanceOf(typeof(IEnumerable<ComakershipBasic>)));

            //Check that the GetAll method was called once
            _MockComakershipRepository.Verify(rep => rep.GetComakerships(), Times.Once);
        }

        [Test]
        public async Task Calling_AcceptApplication_Should_Return_True() {
            Comakership comakership = new Comakership() {
                Id = 1,
                Name = "coole comakership",
                CompanyId = 1,
                Company = new Company() {
                    Id = 1,
                    Name = "coolbedrijf"
                },
                Applications = new List<TeamComakership>() { new TeamComakership() { ComakershipId = 1, TeamId = 1 } }
            };
            _MockComakershipRepository.Setup(rep => rep.AddTeamToComakership(It.IsAny<int>(), It.IsAny<Comakership>())).Returns(Task.FromResult(true));
            _MockComakershipRepository.Setup(rep => rep.GetComakership(It.IsAny<int>())).ReturnsAsync(comakership);

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("Companyid")).Returns(new Claim("Companyid", "1"));

            var result = await _ComakershipService.AcceptApplication(1, 1, claims.Object);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task Calling_GetComakershipsForUser_Should_Return_List() {
            IEnumerable<Comakership> list = new List<Comakership>() {
                new Comakership() {
                    Id = 1,
                    Name = "coole comakership",
                    CompanyId = 1,
                    Company = new Company() {
                        Id = 1,
                        Name = "coolbedrijf"
                    },
                    StudentUsers = new List<UserComakership>() {
                        new UserComakership(){
                            StudentUserId = 1,
                            ComakershipId = 1
                        }
                    },

                }
            };

            _MockComakershipRepository.Setup(rep => rep.GetComakershipsFromUser(It.IsAny<int>())).Returns(Task.FromResult(list));

            Mock<ClaimsIdentity> claims = new Mock<ClaimsIdentity>();
            claims.Setup(c => c.FindFirst("UserId")).Returns(new Claim("UserId", "1"));

            var result = await _ComakershipService.GetComakershipsForUser(claims.Object);

            Assert.NotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TearDown]
        public void TestCleanUp()
        {
            _MockComakershipRepository = null;
            _MockComakershipsList = null;
        }
    }
}