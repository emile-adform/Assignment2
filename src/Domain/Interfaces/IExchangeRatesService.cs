using Domain.DTOs;

namespace Domain.Interfaces;

public interface IExchangeRatesService
{
    Task<List<CurrencyChangeDto>> GetCurrencyChanges(DateTime date);
    Task CleanupAsync();

}
