using Bank.Query.GetBank;
using Bank.Query.GetBanks;
using Bank.Query.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Query.Infrastructure;

public static class QueryHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddQueriesHandler(this IServiceCollection services)
    {
        services.AddTransient<IQueryDispatcher, QueryDispatcher>();

        //Start services registration for existing query handlers
        services.AddTransient<IQueryHandler<GetBankQuery, GetBankQueryReadModel>, GetBankQueryHandler>();
        services.AddTransient<IQueryHandler<GetBanksQuery, GetBanksQueryReadModel>, GetBanksQueryHandler>();
        return services;
    }
}