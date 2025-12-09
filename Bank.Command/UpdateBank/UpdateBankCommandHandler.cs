using Bank.Command.Infrastructure.Interfaces;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Microsoft.Extensions.Logging;

namespace Bank.Command.UpdateBank;

public class UpdateBankCommandHandler : ICommandHandler<UpdateBankCommand>
{
    private readonly CommandDatabaseContext _context;
    private readonly ILogger<UpdateBankCommandHandler> _logger;
    private readonly QueryDatabaseContext _queryContext;

    public UpdateBankCommandHandler(CommandDatabaseContext context, QueryDatabaseContext queryContext,
        ILogger<UpdateBankCommandHandler> logger)
    {
        _context = context;
        _queryContext = queryContext;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateBankCommand command)
    {
        var bankInCommandDb = _context.Banks.FirstOrDefault(b => b.Id == command.Id);
        if (bankInCommandDb != null)
        {
            bankInCommandDb.Name = command.Name;
            bankInCommandDb.Description = command.Description;
            bankInCommandDb.City = command.City;
            bankInCommandDb.Country = command.Country;
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var bankInQueryDb = _queryContext.Banks.FirstOrDefault(b => b.Id == command.Id);
                if (bankInQueryDb != null)
                {
                    bankInQueryDb.Name = command.Name;
                    bankInQueryDb.Description = command.Description;
                    bankInQueryDb.City = command.City;
                    bankInQueryDb.Country = command.Country;
                    await _queryContext.SaveChangesAsync();
                    _logger.LogInformation("Bank updated successfully in both Command and Query databases.");
                }
                else
                {
                    _logger.LogWarning("Bank not found in Query database.");
                }
            }
            else
            {
                _logger.LogError("Failed to update bank in Command database.");
            }
        }
        else
        {
            _logger.LogWarning("Bank not found in Command database.");
        }
    }
}