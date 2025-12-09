namespace Bank.Command.CommandDispatcher;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler =
            _serviceProvider.GetService(typeof(ICommandHandler<TCommand>)) as ICommandHandler<TCommand>;

        if (handler == null)
            throw new InvalidOperationException($"No handler registered for command type {typeof(TCommand).Name}");

        await handler.HandleAsync(command);
    }
}