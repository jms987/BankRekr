using Bank.Command.Infrastructure.Interfaces;

namespace Bank.Command.DeleteBank;

public class DeleteBankCommand : ICommand
{
    public Guid BankId { get; set; }
}