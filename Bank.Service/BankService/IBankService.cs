using Bank.Query.GetBank;
using Bank.Query.GetBanks;
using Bank.Service.DTOs;

namespace Bank.Service.BankService;

public interface IBankService
{
    public Task CreateBankAsync(AddBank bank);
    public Task UpdateBankAsync(UpdateBank bank);
    public Task DeleteBankAsync(Guid bankId);
    public Task<GetBanksQueryReadModel> GetAllBanksAsync();
    public Task<GetBankQueryReadModel> GetBankByIdAsync(Guid bankId);
}