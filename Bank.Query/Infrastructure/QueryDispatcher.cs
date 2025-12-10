using Bank.Query.Infrastructure.Interfaces;

namespace Bank.Query.Infrastructure;

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

        if (service == null)
            throw new InvalidOperationException($"No handler registered for query type {typeof(TQuery).Name}");

        return await service.HandleAsync(query);
    }
}