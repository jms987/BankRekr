using Bank.Command.AddBank;
using Bank.Command.DeleteBank;
using Bank.Command.UpdateBank;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Command.CommandDispatcher;

public static class CommandHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddCommandsHandler(this IServiceCollection services)
    {
        services.AddTransient<ICommandDispatcher, CommandDispatcher>();
        //Start services registration for existing command handlers

        services.AddTransient<ICommandHandler<AddBankCommand>, AddBankCommandHandler>();
        services.AddTransient<ICommandHandler<DeleteBankCommand>, DeleteBankCommandHandler>();
        services.AddTransient<ICommandHandler<UpdateBankCommand>, UpdateBankCommandHandler>();

        return services;
    }
}