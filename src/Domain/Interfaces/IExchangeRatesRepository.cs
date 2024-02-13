using Domain.Entities;

namespace Domain.Interfaces;

public interface IExchangeRatesRepository
{
    Task InsertExchangeRatesAsync(List<ExchangeRateEntity> rates);
    Task<IEnumerable<ExchangeRateEntity>> GetExchangeRatesAsync(DateTime date);
    Task DeleteAll();

}
