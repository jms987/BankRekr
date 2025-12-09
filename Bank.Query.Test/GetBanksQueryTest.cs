using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Bank.Query.GetBank;
using Bank.Query.QueryDispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Query.Test
{
    [TestFixture]
    public class GetBanksQueryTest
    {
        private ServiceProvider _serviceProvider;
        private QueryDatabaseContext _queryContext;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<QueryDatabaseContext>(options =>
                options.UseInMemoryDatabase(databaseName: $"QueryTestDb_{Guid.NewGuid()}"));

            services.AddLogging();

            services.AddQueryHandler();
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
            Assert.AreEqual(2, result.Banks.Count());
            Assert.IsTrue(result.Banks.Any(b => b.Name == "Millenium"));
            Assert.IsTrue(result.Banks.Any(b => b.Name == "PKO BP"));
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
            Assert.AreEqual(0, result.Banks.Count());
        }
    }
}
