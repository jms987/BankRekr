using Bank.Command.Infrastructure.Interfaces;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Microsoft.Extensions.Logging;

namespace Bank.Command.AddBank;

public class AddBankCommandHandler : ICommandHandler<AddBankCommand>
{
    private readonly CommandDatabaseContext _context;
    private readonly ILogger<AddBankCommandHandler> _logger;
    private readonly QueryDatabaseContext _queryContext;

    public AddBankCommandHandler(CommandDatabaseContext context, QueryDatabaseContext queryContext,
        ILogger<AddBankCommandHandler> logger)
    {
        _context = context;
        _queryContext = queryContext;
        _logger = logger;
    }

    public async Task HandleAsync(AddBankCommand command)
    {
        var bankGuid = Guid.NewGuid();
        _context.Banks.Add(new Domain.Bank
        {
            Id = bankGuid,
            Name = command.Name,
            Description = command.Description,
            City = command.City,
            Country = command.Country
        });
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            // Synchronize with Query Database
            _queryContext.Banks.Add(new Domain.Bank
            {
                Id = bankGuid,
                Name = command.Name,
                Description = command.Description,
                City = command.City,
                Country = command.Country
            });
            await _queryContext.SaveChangesAsync();
            _logger.LogInformation("Bank added successfully to both Command and Query databases.");
        }
        else
        {
            _logger.LogError("Failed to add bank to Command database.");
        }
    }
}