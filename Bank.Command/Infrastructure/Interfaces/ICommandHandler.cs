namespace Bank.Command.CommandDispatcher;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}