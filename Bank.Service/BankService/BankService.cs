using Bank.Command.AddBank;
using Bank.Command.DeleteBank;
using Bank.Command.Infrastructure.Interfaces;
using Bank.Command.UpdateBank;
using Bank.Query.GetBank;
using Bank.Query.GetBanks;
using Bank.Query.Infrastructure.Interfaces;
using Bank.Service.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace Bank.Service.BankService;

public class BankService : IBankService
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public BankService(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    public async Task CreateBankAsync(AddBank bank)
    {
        await _commandDispatcher.HandleAsync(new AddBankCommand
        {
            Name = bank.Name,
            Description = bank.Description,
            City = bank.City,
            Country = bank.Country
        });
    }

    public async Task UpdateBankAsync(UpdateBank bank)
    {
        await _commandDispatcher.HandleAsync(new UpdateBankCommand
        {
            Id = bank.Id,
            Name = bank.Name,
            Description = bank.Description,
            City = bank.City,
            Country = bank.Country
        });
    }

    public async Task DeleteBankAsync(Guid bankId)
    {
        await _commandDispatcher.HandleAsync(new DeleteBankCommand
        {
            BankId = bankId
        });
    }

    public Task<GetBanksQueryReadModel> GetAllBanksAsync()
    {
        return _queryDispatcher.HandleAsync<GetBanksQuery, GetBanksQueryReadModel>(new GetBanksQuery());
    }

    public Task<GetBankQueryReadModel> GetBankByIdAsync(Guid bankId)
    {
        return _queryDispatcher.HandleAsync<GetBankQuery, GetBankQueryReadModel>(new GetBankQuery
        {
            BankId = bankId
        });
    }
}

public static class ServiceRegistration
{
    public static IServiceCollection AddServiceRegister(this IServiceCollection services)
    {
        services.AddTransient<IBankService, BankService>();
        return services;
    }
}