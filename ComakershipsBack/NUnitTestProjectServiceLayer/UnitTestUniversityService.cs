using AutoMapper;
using ComakershipsApi.Infrastructure;
using DAL;
using Models;
using Models.ViewModels;
using Moq;
using NUnit.Framework;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnitTestProjectServiceLayer
{
    public class UnitTestUniversityService
    {
        private Mock<IUniversityRepository> _mockUniversityRepository;
        private UniversityService _universityService;
        private List<University> _mockUniversityList;
        private List<UniversityDomainVM> _mockUniversityDomainVMList;
        private IMapper _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockUniversityRepository = new Mock<IUniversityRepository>();

            // setup mapper
            _mockMapper = Mock.Of<IMapper>();

            var mapperConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new MappingProfile());
            });
            _mockMapper = mapperConfig.CreateMapper();

            _universityService = new UniversityService(_mockUniversityRepository.Object, _mockMapper);

            initializeBogusUniversityList();
            initializeBogusUniversityDomainVMList();
        }

        private void initializeBogusUniversityList()
        {
            _mockUniversityList = new List<University>();

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

            _mockUniversityList.Add(u1);
            _mockUniversityList.Add(u2);
        }

        private void initializeBogusUniversityDomainVMList()
        {
            _mockUniversityDomainVMList = new List<UniversityDomainVM>();

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

            _mockUniversityDomainVMList.Add(u1);
            _mockUniversityDomainVMList.Add(u2);
            _mockUniversityDomainVMList.Add(u3);
        }

        /// <summary>
        /// Test for GetAllUniversitiesAsync
        /// </summary>
        [Test]
        public void Calling_GetAllUniversitiesAsync_On_ServiceLayer_Should_Call_UniversityRepository_And_Return_All_mockUniversities()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.GetAllUniversitiesAsync()).ReturnsAsync(_mockUniversityList);

            // Act
            IEnumerable<University> result = _universityService.GetAllUniversitiesAsync().Result;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count(), 2);
            Assert.IsInstanceOf<IEnumerable<University>>(result);

            // Verify
            _mockUniversityRepository.Verify(m => m.GetAllUniversitiesAsync(), Times.Once);
        }

        /// <summary>
        /// Test for GetUniversityByIdAsync
        /// </summary>
        [Test]
        public void Calling_GetUniversityByIdAsync_On_ServiceLayer_Should_Call_UniversityRepository_And_Return_University_If_University_Exists()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.GetUniversityByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => _mockUniversityList.FirstOrDefault(u => u.Id == i));

            // Act
            University university = _universityService.GetUniversityByIdAsync(124).Result;

            // Assert
            Assert.IsNotNull(university);
            Assert.IsInstanceOf<University>(university);
            Assert.AreEqual(university.Id, 124);

            // Verify
            _mockUniversityRepository.Verify(m => m.GetUniversityByIdAsync(It.IsAny<int>()), Times.Once);
        }

        /// <summary>
        /// Test for CheckIfUniversitynameExistsAsync
        /// </summary>
        [Test]
        public void Calling_CheckIfUniversityNameExistsAsync_Should_Call_UniversityRepository_And_Returns_True_If_University_Exists()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.CheckIfUniversitynameExistsAsync(It.IsAny<String>())).ReturnsAsync(true);

            // Act
            Task<bool> isFound = _universityService.CheckIfUniversitynameExistsAsync("Inholland");

            // Assert
            Assert.IsNotNull(isFound);
            Assert.IsTrue(isFound.Result);

            // Verify
            _mockUniversityRepository.Verify(m => m.CheckIfUniversitynameExistsAsync("Inholland"), Times.Once);
        }

        /// <summary>
        /// Test for CheckIfUniversityExistsAsync
        /// </summary>
        [Test]
        public void Calling_CheckIfUniversityExistsAsync_Should_Call_UniversityRepository_And_Returns_True_If_University_Exists()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.CheckIfUniversityExistsAsync(It.IsAny<int>())).Callback(new Action<int>(x =>
            {
                _mockUniversityList.First(u => u.Id == x);
            }
            )).ReturnsAsync(true);

            // Act
            Task<bool> isFound = _universityService.CheckIfUniversityExistsAsync(124);

            // Assert
            Assert.IsNotNull(isFound);
            Assert.IsTrue(isFound.Result);

            // Verify
            _mockUniversityRepository.Verify(m => m.CheckIfUniversityExistsAsync(It.IsAny<int>()), Times.Once);
        }

        /// <summary>
        /// Test for DeleteUniversityByIdAsync
        /// </summary>
        [Test]
        public void Calling_DeleteUniversityByIdAsync_Should_Call_UniversityRepository_And_Returns_True_If_Deleted()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.DeleteUniversityByIdAsync(It.IsAny<int>())).Callback(new Action<int>(x =>
            {
                _mockUniversityList.RemoveAll(u => u.Id == x);
            })).ReturnsAsync(true);

            // Act
            Task<bool> isDeleted = _universityService.DeleteUniversityByIdAsync(66);

            // Assert
            Assert.IsNotNull(isDeleted);
            Assert.IsTrue(isDeleted.Result);

            // Verify
            _mockUniversityRepository.Verify(m => m.DeleteUniversityByIdAsync(It.IsAny<int>()), Times.Once);
        }

        /// <summary>
        /// Test for EditUniversityAsync
        /// </summary>
        [Test]
        public void Calling_EditUniversityAsync_Should_Call_UniversityRepository_And_Returns_True_If_Edited()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.EditUniversityAsync(It.IsAny<University>())).Callback(new Action<University>(x =>
            {
                University university = _mockUniversityList.Find(u => u.Id == x.Id);
                int index = _mockUniversityList.IndexOf(university);
                _mockUniversityList[index] = x;
            })).ReturnsAsync(true);

            UniversityPutVM universityPutVM = new UniversityPutVM()
            {
                Id = 66,
                Name = "InhollandNewName",
            };

            // Act
            Task<bool> isEdited = _universityService.EditUniversityAsync(universityPutVM);

            // Assert
            Assert.IsNotNull(isEdited);
            Assert.IsTrue(isEdited.Result);
            Assert.AreEqual("InhollandNewName",_mockUniversityList.Find(u => u.Id == 66).Name);

            // Verify
            _mockUniversityRepository.Verify(m => m.EditUniversityAsync(It.IsAny<University>()), Times.Once);
        }

        /// <summary>
        /// Test for SaveUniversityAsync
        /// </summary>
        [Test]
        public void Calling_SaveUniversityAsync_Should_Call_UniversityRepository_And_Returns_True_If_Saved()
        {
            // Arrange
            _mockUniversityRepository.Setup(u => u.SaveUniversityAsync(It.IsAny<University>())).Callback(new Action<University>(x =>
            {
                _mockUniversityList.Add(x);
            }
            )).ReturnsAsync(true);


            UniversityPostVM universityPostVM = new UniversityPostVM()
            {
                Name = "BogusUniversity",
                Domain = "BogusDomain.bogus"
            };

            // Act
            Task<bool> isSaved = _universityService.SaveUniversityAsync(universityPostVM);

            // Assert
            Assert.IsNotNull(isSaved);
            Assert.IsTrue(isSaved.Result);
            Assert.AreEqual(3, _mockUniversityList.Count());
            Assert.AreEqual("BogusUniversity", _mockUniversityList.Find(u => u.Name == "BogusUniversity").Name);

            // Verify
            _mockUniversityRepository.Verify(m => m.SaveUniversityAsync(It.IsAny<University>()), Times.Once);
        }
        [TearDown]
        public void TearDown()
        {
            // Arrange
            // Act
            // Assert

            _mockUniversityList = null;
            _mockUniversityDomainVMList = null;
            _mockUniversityRepository = null;
        }
    }


}
