using Domain.DTOs;
using Domain.Interfaces;

namespace Application.Services
{
    public class ExchangeRatesService
    {
        private readonly IExchangeRatesClient _client;
        public ExchangeRatesService(IExchangeRatesClient client)
        {
            _client = client;
        }
        public async Task<ExchangeRates> GetRatesByDateAsync(DateTime date)
        {
            return await _client.GetExchangeRatesByDateAsync(date);
        }
        public async Task<List<CurrencyChangesDto>> GetCurrencyChanges(DateTime date)
        {
            var SelectedDateRates = await GetExchangeRatesByDate(date);
            var PriorDayRates = await GetExchangeRatesByDate(date.AddDays(-1));

            var CurrencyChanges = new List<CurrencyChangesDto>();

            foreach (var currencyRate in SelectedDateRates)
            {
                var priorRate = PriorDayRates.FirstOrDefault(r => r.Currency == currencyRate.Currency)?.Rate;
                var change = (decimal)((currencyRate.Rate - priorRate) / priorRate * 100);
                CurrencyChanges.Add(new CurrencyChangesDto { Change = change, Currency = currencyRate.Currency, Date = currencyRate.Date });
            }

            var orderedList = CurrencyChanges.OrderByDescending(c => c.Change).ToList();
            await InsertIntoDatabase(date, orderedList);
            return orderedList;
        }
    }
}
