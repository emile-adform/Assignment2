
namespace Domain.Entities;

public class ExchangeRateEntity
{
    public string Currency { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public DateTime ExchangeDate { get; set; }
}
