using Domain.DTOs;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IExchangeRatesClient
    {
        Task<ExchangeRates> GetExchangeRatesByDateAsync(DateTime date);
    }
}
