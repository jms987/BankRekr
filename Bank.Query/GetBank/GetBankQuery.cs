using Bank.Query.Infrastructure.Interfaces;

namespace Bank.Query.GetBank;

public class GetBankQuery : IQuery<GetBankQueryReadModel>
{
    public Guid BankId { get; set; }
}