using Bank.Command.Infrastructure;
using Bank.Command.Infrastructure.Interfaces;
using Bank.Command.UpdateBank;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Command.Test;

public class UpdateCommandTest
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
    public async Task Test_UpdateBankCommand_ShouldUpdateBankInCommandDb()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        // Arrange
        var bankId = Guid.NewGuid();
        var bank = new Domain.Bank
        {
            Id = bankId,
            Name = "Old Bank",
            Description = "Old Description",
            City = "Old City",
            Country = "Old Country"
        };
        _commandContext.Banks.Add(bank);
        await _commandContext.SaveChangesAsync();
        var updateCommand = new UpdateBankCommand
        {
            Id = bankId,
            Name = "Updated Bank",
            Description = "Updated Description",
            City = "Updated City",
            Country = "Updated Country"
        };
        // Act
        await dispatcher.HandleAsync(updateCommand);
        // Assert
        var updatedBank = await _commandContext.Banks.FindAsync(bankId);
        Assert.IsNotNull(updatedBank);
        Assert.That(updatedBank!.Name, Is.EqualTo("Updated Bank"));
        Assert.That(updatedBank.Description, Is.EqualTo("Updated Description"));
        Assert.That(updatedBank.City, Is.EqualTo("Updated City"));
        Assert.That(updatedBank.Country, Is.EqualTo("Updated Country"));
    }

    [Test]
    public Task Test_UpdateBankCommand_NonExistentBank_ShouldNotThrow()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        // Arrange
        var updateCommand = new UpdateBankCommand
        {
            Id = Guid.NewGuid(), // Non-existent bank ID
            Name = "NonExistent Bank",
            Description = "This bank does not exist",
            City = "Nowhere",
            Country = "Noland"
        };
        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await dispatcher.HandleAsync(updateCommand));
        return Task.CompletedTask;
    }

    [Test]
    public async Task Test_UpdateBankCommand_ShouldUpdateCommandAndQueryDB()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        var bankId = Guid.NewGuid();
        var bank = new Domain.Bank
        {
            Id = bankId,
            Name = "Old Bank",
            Description = "Old Description",
            City = "Old City",
            Country = "Old Country"
        };
        _commandContext.Banks.Add(bank);
        _queryContext.Banks.Add(bank);
        await _commandContext.SaveChangesAsync();
        await _queryContext.SaveChangesAsync();
        var updateCommand = new UpdateBankCommand
        {
            Id = bankId,
            Name = "Updated Bank",
            Description = "Updated Description",
            City = "Updated City",
            Country = "Updated Country"
        };

        await dispatcher.HandleAsync(updateCommand);
        var updatedCommandBank = await _commandContext.Banks.FirstOrDefaultAsync(b => b.Id == bankId);
        var updatedQueryBank = await _queryContext.Banks.FirstOrDefaultAsync(b => b.Id == bankId);

        Assert.IsNotNull(updatedCommandBank);
        Assert.That(updatedCommandBank!.Name, Is.EqualTo("Updated Bank"));
        Assert.That(updatedCommandBank.Description, Is.EqualTo("Updated Description"));
        Assert.That(updatedCommandBank.City, Is.EqualTo("Updated City"));
        Assert.That(updatedCommandBank.Country, Is.EqualTo("Updated Country"));

        Assert.IsNotNull(updatedQueryBank);
        Assert.That(updatedQueryBank!.Name, Is.EqualTo("Updated Bank"));
        Assert.That(updatedQueryBank.Description, Is.EqualTo("Updated Description"));
        Assert.That(updatedQueryBank.City, Is.EqualTo("Updated City"));
        Assert.That(updatedQueryBank.Country, Is.EqualTo("Updated Country"));
    }
}