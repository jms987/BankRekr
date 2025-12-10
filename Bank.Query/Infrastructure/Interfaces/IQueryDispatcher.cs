namespace Bank.Query.Infrastructure.Interfaces;

public interface IQueryDispatcher
{
    Task<TResult> HandleAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
}