namespace Bank.Command.CommandDispatcher
{
    public class CommandDispatcher: ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task HandleAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(typeof(TCommand));
            var handler = _serviceProvider.GetService(handlerType);
            
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for command type {typeof(TCommand).Name}");
            }
            
            await (Task)handlerType.GetMethod("Handle")!.Invoke(handler, new object[] { command })!;
        }
    }
    public interface ICommandDispatcher
    {
        Task HandleAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public interface ICommand
    {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }

}
