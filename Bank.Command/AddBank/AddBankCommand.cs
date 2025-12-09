using Bank.Command.Infrastructure.Interfaces;

namespace Bank.Command.AddBank;

public class AddBankCommand : ICommand
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}