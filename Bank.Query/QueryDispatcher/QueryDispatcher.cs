using Bank.Query.GetBank;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Query.QueryDispatcher
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var service =
                _serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>)) as IQueryHandler<TQuery, TResult>;
            return await service.HandleAsync(query);
        }
    }


    public interface IQueryDispatcher
    {
        Task<TResult> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }

    public interface IQuery<TResult>
    {
    }

    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }

    public static class QueryHandlerServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryHandler(this IServiceCollection services)
        {
            services.AddTransient<IQueryDispatcher, QueryDispatcher>();

            //Start services registration for existing query handlers
            services.AddTransient<IQueryHandler<GetBankQuery,GetBankQueryReadModel>,GetBankQueryHandler>();
            services.AddTransient<IQueryHandler<GetBanksQuery,GetBanksQueryReadModel>,GetBanksQueryHandler>();
            return services;
        }
    }


}
