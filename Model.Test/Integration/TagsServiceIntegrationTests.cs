using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Model.DataAccess;
using Model.DataAccess.Context;
using Model.DataAccess.Entities;
using Model.DataAccess.Interfaces;
using Model.Services;
using Model.Services.Interfaces;
using Model.Features.Queries.GetTags;

[TestFixture]
public class TagsServiceIntegrationTests
{
    private ServiceProvider _serviceProvider = default!;
    private ITagsService _tagsService = default!;
    private TagsContext _context = default!;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TagsContext>(options =>
            options.UseSqlite("Data Source=:memory:"));

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Mock logger
        var loggerMock = new Mock<ILogger<TagsService>>();
        services.AddSingleton(loggerMock.Object);

        var mockFactory = new Mock<IHttpClientFactory>();
        services.AddSingleton(mockFactory.Object);

        var mockBuilder = new Mock<IRequestMessageBuilder>();
        services.AddSingleton(mockBuilder.Object);

        services.AddTransient<ITagsDao, TagsDao>();
        services.AddTransient<ITagsService, TagsService>();

        _serviceProvider = services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<TagsContext>();
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _tagsService = _serviceProvider.GetRequiredService<ITagsService>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task GetTags_ShouldReturnTags_WhenDatabaseHasData()
    {
        _context.Tags.Add(new Tag { Name = "csharp", Count = 100 });
        _context.Tags.Add(new Tag { Name = "dotnet", Count = 50 });
        await _context.SaveChangesAsync();

        var query = new GetTagsQuery
        {
            Page = 1,
            PageSize = 10,
            Sort = "name",
            Order = "asc"
        };

        var tags = await _tagsService.GetTags(query, CancellationToken.None);

        Assert.IsNotNull(tags);
        Assert.AreEqual(2, tags.Count);
        Assert.AreEqual("csharp", tags[0].Name);
    }
}
