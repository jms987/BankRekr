using Bank.Infrastructure.QueryDatabase;
using Bank.Query.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Query.GetBank;

public class GetBankQueryHandler : IQueryHandler<GetBankQuery, GetBankQueryReadModel>
{
    private readonly QueryDatabaseContext _context;
    private readonly ILogger<GetBankQueryHandler> _logger;

    public GetBankQueryHandler(QueryDatabaseContext context, ILogger<GetBankQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetBankQueryReadModel> HandleAsync(GetBankQuery query)
    {
        var cache = await _context.Banks.FirstOrDefaultAsync(b => b.Id == query.BankId);
        if (cache != null)
        {
            _logger.LogInformation("Bank found in query database cache with Id: {BankId}", query.BankId);
            return new GetBankQueryReadModel { Bank = cache };
        }

        _logger.LogWarning("Bank not found in both query and command databases with Id: {BankId}", query.BankId);
        return new GetBankQueryReadModel();
    }
}