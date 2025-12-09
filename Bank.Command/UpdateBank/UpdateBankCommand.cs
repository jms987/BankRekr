using Bank.Command.Infrastructure.Interfaces;

namespace Bank.Command.UpdateBank;

public class UpdateBankCommand : ICommand
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}