using Bank.Command.DeleteBank;
using Bank.Command.Infrastructure;
using Bank.Command.Infrastructure.Interfaces;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Command.Test;

public class DeleteCommandTest
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
    public async Task Test_DeleteBankCommand_ShouldRemoveBankFromCommandDb()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        // Arrange
        var bankId = Guid.NewGuid();
        var bank = new Domain.Bank
        {
            Id = bankId,
            Name = "Test Bank",
            Description = "Bank to be deleted",
            City = "Test City",
            Country = "Test Country"
        };
        var bank2 = new Domain.Bank
        {
            Id = Guid.NewGuid(),
            Name = "Test Bank2",
            Description = "Bank to not be deleted",
            City = "Test City2",
            Country = "Test Country2"
        };
        _commandContext.Banks.AddRange(bank, bank2);
        await _commandContext.SaveChangesAsync();
        // Act
        var deleteCommand = new DeleteBankCommand { BankId = bankId };
        await dispatcher.HandleAsync(deleteCommand);
        // Assert
        var deletedBank = await _commandContext.Banks.FindAsync(bankId);
        Assert.IsNull(deletedBank);
    }

    [Test]
    public async Task Test_DeleteBankCommand_ShouldRemoveBankFromCommandAndQueryDb()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        // Arrange
        var bankId = Guid.NewGuid();
        var bank = new Domain.Bank
        {
            Id = bankId,
            Name = "Test Bank",
            Description = "Bank to be deleted",
            City = "Test City",
            Country = "Test Country"
        };
        var bank2 = new Domain.Bank
        {
            Id = Guid.NewGuid(),
            Name = "Test Bank2",
            Description = "Bank to not be deleted",
            City = "Test City2",
            Country = "Test Country2"
        };
        _queryContext.Banks.AddRange(bank, bank2);
        _commandContext.Banks.AddRange(bank, bank2);
        await _queryContext.SaveChangesAsync();
        await _commandContext.SaveChangesAsync();
        // Act
        var deleteCommand = new DeleteBankCommand { BankId = bankId };
        await dispatcher.HandleAsync(deleteCommand);
        // Assert
        var deletedBank = await _queryContext.Banks.FindAsync(bankId);
        Assert.IsNull(deletedBank);
    }


    [Test]
    public Task Test_DeleteBankCommand_NonExistentBank()
    {
        var dispatcher = _serviceProvider.GetRequiredService<ICommandDispatcher>();
        var nonExistentBankId = Guid.NewGuid();
        var deleteCommand = new DeleteBankCommand { BankId = nonExistentBankId };
        Assert.DoesNotThrowAsync(async () => await dispatcher.HandleAsync(deleteCommand));
        return Task.CompletedTask;
    }
}