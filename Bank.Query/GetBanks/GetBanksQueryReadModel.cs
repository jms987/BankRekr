namespace Bank.Query.GetBanks;

public class GetBanksQueryReadModel
{
    public IEnumerable<Domain.Bank> Banks { get; set; } = Array.Empty<Domain.Bank>();
}