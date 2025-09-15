using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.DataTransfer;
using Model.Features.Commands.RefreshTags;
using Model.Features.Queries.GetTags;
using Model.Services;
using Moq;
using Moq.Protected;

namespace Model.Test.Unit
{
    [TestFixture]
    public class TagsServiceTests
    {
        private Mock<ITagsDao> _tagsDaoMock;
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IRequestMessageBuilder> _requestMessageBuilderMock;
        private TagsService _service;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _tagsDaoMock = new Mock<ITagsDao>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();
            _requestMessageBuilderMock = new Mock<IRequestMessageBuilder>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(f => f.CreateClient("StackOverflow"))
                .Returns(_httpClient);
            _configurationMock.Setup(c => c.GetSection("SO")["ApiKey"])
                .Returns("dummyApiKey");

            _service = new TagsService(
                _tagsDaoMock.Object,
                _httpClientFactoryMock.Object,
                _configurationMock.Object,
                _requestMessageBuilderMock.Object);
        }
        
        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }


        private HttpResponseMessage CreateHttpResponse<T>(T value)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(value)
            };
        }

        [Test]
        public async Task GetTags_ReturnsTagsFromDao_WhenDaoHasTags()
        {
            // Arrange
            var tags = new List<Tag> { new Tag { Name = "c#", Count = 100 } };
            var query = new GetTagsQuery();
            _tagsDaoMock.Setup(x => x.GetTags(query)).ReturnsAsync(tags);

            // Act
            var result = await _service.GetTags(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(tags, result);
        }

        [Test]
        public async Task GetTags_ReturnsTagsFromApi_WhenDaoIsEmpty()
        {
            // Arrange
            var query = new GetTagsQuery { Page = 1, Order = "desc", PageSize = 10, Sort = "popular" };
            _tagsDaoMock.Setup(x => x.GetTags(query)).ReturnsAsync(new List<Tag>());
            var apiTags = new List<Tag> { new Tag { Name = "c#", Count = 100 } };
            var tagsResponse = new TagsResponse { Items = apiTags };

            var request = new HttpRequestMessage(HttpMethod.Get, "http://dummy");
            _requestMessageBuilderMock.Setup(x => x.BuildGet(It.IsAny<string>())).Returns(request);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(CreateHttpResponse(tagsResponse));

            // Act
            var result = await _service.GetTags(query, CancellationToken.None);

            // Assert
            Assert.AreEqual(apiTags.Count, result.Count);
            Assert.AreEqual(apiTags[0].Name, result[0].Name);
        }

        [Test]
        public async Task UpdateTags_AddsMissingTagsAndSaves_WhenTagsCountLessThan1000()
        {
            // Arrange
            var existingTags = new List<Tag> { new Tag { Name = "tag1", Count = 10 } };
            _tagsDaoMock.Setup(x => x.GetAllTags()).ReturnsAsync(existingTags);

            var apiTags = new List<Tag>
            {
                new Tag { Name = "tag1", Count = 10 },
                new Tag { Name = "tag2", Count = 20 }
            };
            var tagsResponse = new TagsResponse { Items = apiTags };

            _requestMessageBuilderMock
                .Setup(x => x.BuildGet(It.IsAny<string>()))
                .Returns((string url) => new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/tags"));

            // Setup 10 times for 10 pages
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => CreateHttpResponse(tagsResponse));

            _tagsDaoMock.Setup(x => x.SaveTags(It.IsAny<List<Tag>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.UpdateTags();

            // Assert
            _tagsDaoMock.Verify(x => x.SaveTags(It.Is<List<Tag>>(l => l.Exists(t => t.Name == "tag2"))), Times.Once);
        }

        [Test]
        public async Task RefreshTags_DeletesAllTagsAndSavesNewTags()
        {
            // Arrange
            var command = new RefreshTagsCommand { Page = 1, Order = "desc", PageSize = 10, Sort = "popular" };
            _tagsDaoMock.Setup(x => x.DeleteAllTags()).Returns(Task.CompletedTask);

            var apiTags = new List<Tag> { new Tag { Name = "tagA", Count = 50 } };
            var tagsResponse = new TagsResponse { Items = apiTags };

            _requestMessageBuilderMock
                .Setup(x => x.BuildGet(It.IsAny<string>()))
                .Returns(() => new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/tags"));

            // Setup 10 times for 10 pages
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => CreateHttpResponse(tagsResponse));

            _tagsDaoMock.Setup(x => x.SaveTags(It.IsAny<List<Tag>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.RefreshTags(command);

            // Assert
            _tagsDaoMock.Verify(x => x.DeleteAllTags(), Times.Once);
            _tagsDaoMock.Verify(x => x.SaveTags(It.IsAny<List<Tag>>()), Times.Once);
            Assert.AreEqual(10, result.Count); // 10 pages, each returns 1 tag
        }

        [Test]
        public void CalculatePercent_SetsPercentageCorrectly()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Name = "tag1", Count = 1 },
                new Tag { Name = "tag2", Count = 3 }
            };

            // Act (use reflection to invoke private method)
            var method = typeof(TagsService).GetMethod("CalculatePercent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            method.Invoke(null, new object[] { tags });

            // Assert
            Assert.AreEqual(25, tags[0].Percentage, 0.01);
            Assert.AreEqual(75, tags[1].Percentage, 0.01);
        }
    }
}