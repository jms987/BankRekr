namespace Bank.Command.Infrastructure.Interfaces;

public interface ICommandDispatcher
{
    Task HandleAsync<TCommand>(TCommand command) where TCommand : ICommand;
}