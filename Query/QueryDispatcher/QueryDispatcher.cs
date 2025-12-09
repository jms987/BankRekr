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

    public interface IQueryHandler<TQuery, IQueryResponse> where TQuery : IQuery<TResult>
    {
        Task<IQueryResponse> HandleAsync(TQuery query);
    }


}
