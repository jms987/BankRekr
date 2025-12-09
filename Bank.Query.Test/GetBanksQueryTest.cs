using Bank.Infrastructure.QueryDatabase;
using Bank.Query.GetBanks;
using Bank.Query.Infrastructure;
using Bank.Query.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Query.Test;

[TestFixture]
public class GetBanksQueryTest
{
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddDbContext<QueryDatabaseContext>(options =>
            options.UseInMemoryDatabase($"QueryTestDb_{Guid.NewGuid()}"));

        services.AddLogging();

        services.AddQueriesHandler();
        _serviceProvider = services.BuildServiceProvider();

        _queryContext = _serviceProvider.GetRequiredService<QueryDatabaseContext>();

        _queryContext.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _queryContext?.Database.EnsureDeleted();
        _queryContext?.Dispose();
        _serviceProvider?.Dispose();
    }

    private ServiceProvider _serviceProvider;
    private QueryDatabaseContext _queryContext;

    [Test]
    public async Task Test_GetBanksQuery_ShouldReturnAllBanksFromQueryDb()
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<GetBanksQuery, GetBanksQueryReadModel>>();
        // Arrange
        var bank1 = new Domain.Bank
        {
            Id = Guid.NewGuid(),
            Name = "Millenium",
            Description = "Największy bank w Polsce",
            City = "Warszawa",
            Country = "Polska"
        };
        var bank2 = new Domain.Bank
        {
            Id = Guid.NewGuid(),
            Name = "PKO BP",
            Description = "Drugi największy bank w Polsce",
            City = "Kraków",
            Country = "Polska"
        };
        _queryContext.Banks.AddRange(bank1, bank2);
        await _queryContext.SaveChangesAsync();
        // Act
        var query = new GetBanksQuery();
        var result = await handler.HandleAsync(query);
        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Banks.Count(), Is.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result.Banks.Any(b => b.Name == "Millenium"), Is.True);
            Assert.That(result.Banks.Any(b => b.Name == "PKO BP"), Is.True);
        });
    }


    [Test]
    public async Task Test_GetBanksQuery_ShouldReturnNoBanksFromQueryDb()
    {
        var handler = _serviceProvider.GetRequiredService<IQueryHandler<GetBanksQuery, GetBanksQueryReadModel>>();

        // Act
        var query = new GetBanksQuery();
        var result = await handler.HandleAsync(query);
        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Banks.Count(), Is.EqualTo(0));
    }
}