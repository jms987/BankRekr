using Bank.Command.AddBank;
using Bank.Command.Infrastructure;
using Bank.Command.Infrastructure.Interfaces;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Command.Test;

public class AddCommandTest
{
    private CommandDatabaseContext _commandContext;
    private QueryDatabaseContext _queryContext;
    private ServiceProvider _serviceProvider;

    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddDbContext<QueryDatabaseContext>(options =>
            options.UseInMemoryDatabase($"QueryTestDb_{Guid.NewGuid()}"));

        services.AddDbContext<CommandDatabaseContext>(options =>
            options.UseInMemoryDatabase($"CommandTestDb_{Guid.NewGuid()}"));

        services.AddLogging();

        services.AddCommandsHandler();
        _serviceProvider = services.BuildServiceProvider();

        _queryContext = _serviceProvider.GetRequiredService<QueryDatabaseContext>();
        _commandContext = _serviceProvider.GetRequiredService<CommandDatabaseContext>();
        _queryContext.Database.EnsureCreated();
        _commandContext.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _queryContext?.Database.EnsureDeleted();
        _commandContext?.Database.EnsureDeleted();
        _queryContext?.Dispose();
        _commandContext?.Dispose();
        _serviceProvider?.Dispose();
    }

    [Test]
    public async Task Test_AddBankCommand_ShouldAddBankToCommandDb()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        // Arrange
        var command = new AddBankCommand
        {
            Name = "New Bank",
            Description = "Newly added bank",
            City = "New City",
            Country = "New Country"
        };
        // Act
        await dispatcher.HandleAsync(command);
        // Assert
        var addedBank = await _commandContext.Banks.FirstOrDefaultAsync();
        Assert.IsNotNull(addedBank);
        Assert.That(addedBank!.Name, Is.EqualTo("New Bank"));
        Assert.That(addedBank.Description, Is.EqualTo("Newly added bank"));
        Assert.That(addedBank.City, Is.EqualTo("New City"));
        Assert.That(addedBank.Country, Is.EqualTo("New Country"));
    }

    [Test]
    public async Task Test_AddBankCommand_ShouldAddBankToCommandAndQueryDb()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        // Arrange
        var command = new AddBankCommand
        {
            Name = "New Bank",
            Description = "Newly added bank",
            City = "New City",
            Country = "New Country"
        };
        // Act
        await dispatcher.HandleAsync(command);
        // Assert
        var addedBank = await _commandContext.Banks.FirstOrDefaultAsync();
        Assert.IsNotNull(addedBank);
        Assert.That(addedBank!.Name, Is.EqualTo("New Bank"));
        Assert.That(addedBank.Description, Is.EqualTo("Newly added bank"));
        Assert.That(addedBank.City, Is.EqualTo("New City"));
        Assert.That(addedBank.Country, Is.EqualTo("New Country"));

        var addedBankInQueryDb = await _queryContext.Banks.FirstOrDefaultAsync(b => b.Id == addedBank.Id);
        Assert.IsNotNull(addedBankInQueryDb);
        Assert.That(addedBankInQueryDb!.Name, Is.EqualTo("New Bank"));
        Assert.That(addedBankInQueryDb.Description, Is.EqualTo("Newly added bank"));
        Assert.That(addedBankInQueryDb.City, Is.EqualTo("New City"));
        Assert.That(addedBankInQueryDb.Country, Is.EqualTo("New Country"));
    }
}