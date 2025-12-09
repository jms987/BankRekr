namespace Bank.Service.DTOs;

public class AddBank
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}