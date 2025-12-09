namespace Bank.Command.CommandDispatcher;

public interface ICommandDispatcher
{
    Task HandleAsync<TCommand>(TCommand command) where TCommand : ICommand;
}