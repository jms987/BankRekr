namespace Bank.Command.Infrastructure.Interfaces;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}