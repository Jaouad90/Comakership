using DAL;
using Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace NUnitTestProjectRepository
{
    public class UnitTestComakershipRepo
    {
        private List<Comakership> _MockComakershipsList;
        private Mock<IComakershipRepository> _MockComakershipRepository;
        private IComakershipRepository _ComakershipRepository;


        [SetUp]
        public void Setup()
        {            
            Comakership ComakershipOne = new Comakership { Id = 1, Name = "Create an Enrichment Centre", Description = "We are in need of a new Aperture Science Enrichment Center", ComakershipStatusId = 1, Credits = false, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };
            Comakership ComakershipTwo = new Comakership { Id = 2, Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", ComakershipStatusId = 1, Credits = true, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };

            _MockComakershipsList = new List<Comakership> { ComakershipOne, ComakershipTwo };

            _MockComakershipRepository = new Mock<IComakershipRepository>();

            // Get all           
            _MockComakershipRepository.Setup(rep => rep.GetComakerships()).Returns(Task.FromResult<IEnumerable<Comakership>>(_MockComakershipsList));

            // Get one by id
            _MockComakershipRepository.Setup(rep => rep.GetComakership(It.Is<int>(i => i == 1 || i == 2 || i == 3 || i == 4))).Returns<int>(r => Task.FromResult<Comakership>(new Comakership
            {
                Id = r,
                Name = "Create an Enrichment Centre",
                Description = "We are in need of a new Aperture Science Enrichment Center",
                ComakershipStatusId = 1,
                Credits = false,
                Bonus = true,
                CompanyId = 1,
                CreatedAt = DateTime.Parse("10-10-2020")
            }));

            // Create
            _MockComakershipRepository.Setup(rep => rep.Add(It.IsAny<Comakership>())).Callback(new Action<Comakership>(x =>
            {
                _MockComakershipsList.Add(x);
            }));

            // Update
            _MockComakershipRepository.Setup(rep => rep.Update(It.IsAny<Comakership>())).Callback(new Action<Comakership>(x =>
            {
                var found = _MockComakershipsList.Find(c => c.Id == x.Id);
                found.Name = x.Name;
                found.Description = x.Description;
            }));

            // Delete
            _MockComakershipRepository.Setup(rep => rep.DeleteComakership(It.IsAny<Comakership>())).Callback(new Action<Comakership>(x =>
            {
                _MockComakershipsList.RemoveAll(c => c.Id == x.Id);
            }));

            _ComakershipRepository = _MockComakershipRepository.Object;
        }

        [Test]
        public void GetComakerships_Should_Return_All_MockListComakerships()
        {
            //Arrange 

            //Act
            var test = _ComakershipRepository.GetComakerships().Result.ToList();            

            //Assert
            Assert.AreEqual(2, test.Count);
        }

        [Test]
        public void GetComakership_Should_Return_Correct_Comakership()
        {
            // Arrange

            //Act
            Comakership testComakership = _ComakershipRepository.GetComakership(4).Result;

            //Assert
            Assert.AreEqual(4, testComakership.Id);
        }

        [Test]
        public void CreateComakership_Should_Return_Increased_MockListComakerships()
        {
            // Arrange
            Comakership comakership = new Comakership { Id = 3, Name = "Lorem ipsum", Description = "On of our test-subjects ran of with our portal gun, we need a new one", ComakershipStatusId = 1, Credits = true, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };

            //Act
            _ComakershipRepository.Add(comakership);
            var after = (IList<Comakership>)_ComakershipRepository.GetComakerships().Result;

            //Assert
            Assert.AreEqual(3, after.Count);
        }

        [Test]
        public void Update_Should_ChangeComakership()
        {
            // Arrange
            Comakership testComakership = new Comakership { Id = 2, Name = "New and updated name!", Description = "New and updated description!", ComakershipStatusId = 1, Credits = true, Bonus = true, CompanyId = 1, CreatedAt = DateTime.Parse("10-10-2020") };

            //Act
            _ComakershipRepository.Update(testComakership);

            //Assert
            Assert.AreEqual("New and updated name!", _MockComakershipsList[1].Name);
            Assert.AreEqual("New and updated description!", _MockComakershipsList[1].Description);
        }

        [Test]
        public void Delete_Should_Return_Decreased_MockListComakerships()
        {
            // Arrange
            Comakership testComakership = _MockComakershipsList.First(i => i.Id == 1);

            //Act
            _ComakershipRepository.DeleteComakership(testComakership);

            //Assert
            Assert.AreEqual(1, _MockComakershipsList.Count);
        }



        [TearDown]
        public void TestCleanUp()
        {
            _MockComakershipRepository = null;
            _MockComakershipsList = null;
            _ComakershipRepository = null;
        }
    }
}