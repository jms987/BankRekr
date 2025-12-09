using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Bank.Query.GetBank;
using Bank.Query.QueryDispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Query.Test
{

    [TestFixture]
    public class GetBankTest
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
        public async Task Test_GetBankById_ShouldReturnBankFromQueryCache_WhenBankExistsInQueryDb()
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<GetBankQuery, GetBankQueryReadModel>>();
            // Arrange
            var bankId = Guid.NewGuid();
            var bank = new Domain.Bank
            {
                Id = bankId,
                Name = "Millenium",
                Description = "Największy bank w Polsce",
                City = "Warszawa",
                Country = "Polska"
            };

            await _queryContext.Banks.AddAsync(bank);
            await _queryContext.SaveChangesAsync();

            var query = new GetBankQuery { BankId = bankId };

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Bank, Is.Not.Null);
            Assert.That(result.Bank.Id, Is.EqualTo(bankId));
            Assert.That(result.Bank.Name, Is.EqualTo("Millenium"));
            Assert.That(result.Bank.City, Is.EqualTo("Warszawa"));
            Assert.That(result.Bank.Country, Is.EqualTo("Polska"));
        }

        [Test]
        public async Task Test_GetBankById_ShouldReturnNoBank_WhenBankNotExistsInDb()
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<GetBankQuery, GetBankQueryReadModel>>();
            // Arrange
            var bankId = Guid.NewGuid();
            var bank = new Domain.Bank
            {
                Id = bankId,
                Name = "Millenium",
                Description = "Największy bank w Polsce",
                City = "Warszawa",
                Country = "Polska"
            };

            await _queryContext.Banks.AddAsync(bank);
            await _queryContext.SaveChangesAsync();

            var query = new GetBankQuery { BankId = Guid.NewGuid() };

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.That(result.Bank, Is.Null);
        }
    }
}
