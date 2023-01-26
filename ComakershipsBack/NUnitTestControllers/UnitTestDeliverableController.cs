using ComakershipsApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Moq;
using NUnit.Framework;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTestControllers
{
    class UnitTestDeliverableController
    {
        private Mock<IDeliverableService> _MockDeliverableService;
        private Mock<IComakershipService> _MockComakershipService;
        private List<ComakershipBasic> _MockComakershipList;
        private ILogger<DeliverableController> _MockLogger;

        [SetUp]
        public void Setup()
        {
            // creating new Mock
            _MockDeliverableService = new Mock<IDeliverableService>();
            _MockComakershipService = new Mock<IComakershipService>();

            // fill comakerships
            InitializeBogusComakershipList();

            // setup logger
            _MockLogger = Mock.Of<ILogger<DeliverableController>>();

            // Checking if the comakership exist in the mocked comakership list.
            _MockDeliverableService.Setup(c => c.checkIfComakershipExistAsync(It.IsIn<int>(1, 2))).Callback(new Action<int>(x =>
             {
                 ComakershipBasic c = _MockComakershipList.Find(c => c.Id == x);
             })).ReturnsAsync(true);



        }

        public void AlwaysPass()
        {
            Assert.Pass();
        }

        //[Test]
        //public async Task Upload_Valid_Deliverable_For_Valid_Comakership_Should_Return_Ok()
        //{
        //    // Arrange
        //    var httpRequestMessage = Mock.Of<HttpRequestMessage>();
        //    httpRequestMessage.Method = HttpMethod.Post;
        //    httpRequestMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("aaaa")));
        //    httpRequestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

        //    httpRequestMessage.Content.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue("boundary", "----WebKitFormBoundaryOKeVZNlAIs625X1D"));


        //    //httpRequestMessage.Headers.Add("Content-Type", "multipart/form-data");
        //    //httpRequestMessage.Content = new StringContent(data, Encoding.UTF8, "multipart/form-data");
        //    //httpRequestMessage.Content.Headers.Add("Content-Type", "multipart/form-data");
        //    httpRequestMessage.Content.Headers.ContentLength = 1234;
        //    var DeliverableController = new DeliverableController(_MockLogger, _MockDeliverableService.Object);

        //    // Act
        //    var okResponse = await DeliverableController.UploadDeliverable(httpRequestMessage, 1);

        //    // Assert
        //    Assert.NotNull(okResponse);
        //    Assert.True(okResponse is OkObjectResult);
        //}

        //[Test]
        //public async Task Upload_Valid_Deliverable_For_Valid_Comakership_Should_Return_Ok2()
        //{
        //    // Arrange
        //    var request = new HttpRequestMessage();
        //    request.Method = HttpMethod.Post;
        //    var content = CreateFakeMultiPartFormData();

        //    //content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
        //    //{
        //    //    FileName = "example.png",
        //    //};


        //    //var fileContent = new ByteArrayContent(new byte[100]);
        //    //fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("fileUpload")
        //    //{
        //    //    FileName = "example.png"
        //    //};

        //    //content.Add(fileContent);
        //    request.Content = content;
            
        //    //request.Content.Headers.ContentDisposition.FileName = "example2.png";

        //    var DeliverableController = new DeliverableController(_MockLogger, _MockDeliverableService.Object);

        //    // Act
        //    var okResponse = await DeliverableController.UploadDeliverable(request, 1);

        //    // Assert
        //    Assert.NotNull(okResponse);
        //    Assert.True(okResponse is OkObjectResult);
        //}

        //[Test]
        //public async Task Upload_Valid_Deliverable_For_Valid_Comakership_Should_Return_Ok3()
        //{
        //    // Arrange
        //    var request = Mock.Of<HttpRequestMessage>();
        //    request.Method = HttpMethod.Post;
        //    var content = CreateFakeMultiPartFormData();

        //    byte[] data = { 1, 2, 3, 4, 5 };
        //    var contentStream = new MemoryStream();
        //    //content.Add(fileContent);
        //    request.Content = content;

        //    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content.ToString()));
        //    //request.Content.Headers.ContentDisposition.FileName = "example2.png";

        //    var DeliverableController = new DeliverableController(_MockLogger, _MockDeliverableService.Object);

        //    // Act
        //    var okResponse = await DeliverableController.UploadDeliverable(request, 1);

        //    // Assert
        //    Assert.NotNull(okResponse);
        //    Assert.True(okResponse is OkObjectResult);

        //}

        //[Test]
        //public async Task Upload_Invalid_Deliverable_For_Valid_Comakership_Should_Return_Ok()
        //{
        //    // Arrange

        //    // Act

        //    // Assert
        //}

        [Test]
        public async Task Upload_Valid_Deliverable_But_No_Valid_Comakership_Should_Return_BadRequest()
        {
            // Arrange
            var httpRequestMessage = Mock.Of<HttpRequestMessage>();
            var DeliverableController = new DeliverableController(_MockLogger, _MockDeliverableService.Object, _MockComakershipService.Object);

            // Act
            var response = await DeliverableController.UploadDeliverable(httpRequestMessage, 5);
            var badRequestResponse = response as ObjectResult;

            // Assert
            Assert.NotNull(badRequestResponse);
            Assert.True(badRequestResponse is BadRequestObjectResult);
        }

        private void InitializeBogusComakershipList()
        {
            _MockComakershipList = new List<ComakershipBasic>();

            ComakershipBasic Comakership1 = new ComakershipBasic { Id = 1, Name = "Create an Enrichment Centre", Description = "We are in need of a new Aperture Science Enrichment Center", Credits = false, Bonus = true, Skills = null, CreatedAt = DateTime.Parse("10-10-2020") };
            ComakershipBasic Comakership2 = new ComakershipBasic { Id = 2, Name = "Create a Portal Gun", Description = "On of our test-subjects ran of with our portal gun, we need a new one", Credits = true, Bonus = true, Skills = null, CreatedAt = DateTime.Parse("10-10-2020") };

            _MockComakershipList.Add(Comakership1);
            _MockComakershipList.Add(Comakership2);
        }

        //private MultipartFormDataContent CreateFakeMultiPartFormData()
        //{
        //    byte[] data = { 1, 2, 3, 4, 5 };
        //    ByteArrayContent byteContent = new ByteArrayContent(data);
        //    StringContent stringContent = new StringContent(
        //        "aaaa bbbb ccccc dddd",
        //        System.Text.Encoding.UTF8);


        //    MultipartFormDataContent multipartContent = new MultipartFormDataContent { byteContent, stringContent };
        //    multipartContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
        //    {
        //        FileName = "example.png",
        //        FileNameStar = "example.png",
        //        Name = "example"
        //    };

        //    var dataStream = new MemoryStream(data);

        //    //multipartContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filename", "example3.png"));

        //    //var header = new ContentDispositionHeaderValue("form-data");
        //    //header.Name = "example";
        //    //header.FileName = "example.png";
        //    //header.FileNameStar = "example.png";
        //    //multipartContent.Headers.ContentDisposition = header;

        //    return multipartContent;
        //}
    }
}
