using Bank.Command.Infrastructure.Interfaces;
using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Microsoft.Extensions.Logging;

namespace Bank.Command.DeleteBank;

public class DeleteBankCommandHandler : ICommandHandler<DeleteBankCommand>
{
    private readonly CommandDatabaseContext _context;
    private readonly ILogger<DeleteBankCommandHandler> _logger;
    private readonly QueryDatabaseContext _queryContext;

    public DeleteBankCommandHandler(CommandDatabaseContext context, QueryDatabaseContext queryContext,
        ILogger<DeleteBankCommandHandler> logger)
    {
        _context = context;
        _queryContext = queryContext;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteBankCommand command)
    {
        var bankInCommandDb = _context.Banks.FirstOrDefault(b => b.Id == command.BankId);
        if (bankInCommandDb != null)
        {
            _context.Banks.Remove(bankInCommandDb);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var bankInQueryDb = _queryContext.Banks.FirstOrDefault(b => b.Id == command.BankId);
                if (bankInQueryDb != null)
                {
                    _queryContext.Banks.Remove(bankInQueryDb);
                    await _queryContext.SaveChangesAsync();
                    _logger.LogInformation("Bank deleted successfully from both Command and Query databases.");
                }
                else
                {
                    _logger.LogWarning("Bank not found in Query database.");
                }
            }
            else
            {
                _logger.LogError("Failed to delete bank from Command database.");
            }
        }
        else
        {
            _logger.LogWarning("Bank not found in Command database.");
        }
    }
}