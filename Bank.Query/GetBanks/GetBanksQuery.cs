using Bank.Infrastructure.CommandDatabase;
using Bank.Infrastructure.QueryDatabase;
using Bank.Query.QueryDispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bank.Query.GetBank
{
    public class GetBanksQuery : IQuery<GetBanksQueryReadModel>
    {
    }

    public class GetBanksQueryHandler : IQueryHandler<GetBanksQuery, GetBanksQueryReadModel>
    {
        private readonly QueryDatabaseContext _context;
        private readonly ILogger<GetBankQueryHandler> _logger;

        public GetBanksQueryHandler(QueryDatabaseContext context, ILogger<GetBankQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetBanksQueryReadModel> HandleAsync(GetBanksQuery query)
        {
            var result = new GetBanksQueryReadModel();
            result.Banks = await _context.Banks.ToListAsync();
            _logger.LogInformation("Retrieved {Count} banks from query database.", result.Banks.Count());
            return result;
        }
    }

    public class GetBanksQueryReadModel
    {
        public IEnumerable<Domain.Bank> Banks { get; set; }
    }
}
