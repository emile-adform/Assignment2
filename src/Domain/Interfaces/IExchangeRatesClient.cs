using Domain.DTOs;

namespace Domain.Interfaces;

public interface IExchangeRatesClient
{
    Task<ExchangeRates> GetExchangeRatesByDateAsync(DateTime date);
}
