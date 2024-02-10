
namespace Domain.DTOs;

public class CurrencyChangeDto
{
    public string Currency { get; set; } = string.Empty;
    public decimal Change {  get; set; }
    public DateTime ExchangeDate { get; set; }
}
