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
        public async Task<List<CurrencyChangeDto>> GetCurrencyChanges(DateTime date)
        {
            var SelectedDateRates = await GetExchangeRatesByDate(date);
            var PriorDayRates = await GetExchangeRatesByDate(date.AddDays(-1));

            var CurrencyChanges = new List<CurrencyChangeDto>();

            foreach (var currencyRate in SelectedDateRates)
            {
                var priorRateInfo = PriorDayRates.First(r => r.Currency == currencyRate.Currency);

                decimal change = ((currencyRate.Rate / currencyRate.Quantity) - (priorRateInfo.Rate / priorRateInfo.Quantity)) 
                    / priorRateInfo.Rate * 100;

                CurrencyChanges.Add(new CurrencyChangeDto 
                { 
                    Change = change, 
                    Currency = currencyRate.Currency, 
                    ExchangeDate = currencyRate.ExchangeDate 
                });
            }

            var orderedList = CurrencyChanges.OrderByDescending(c => c.Change).ToList();
            return orderedList;
        }
        public async Task<List<ExchangeRateDto>> GetExchangeRatesByDate(DateTime date)
        {
            var result = await _client.GetExchangeRatesByDateAsync(date);

            return (from rate in result.Rates
                    select new ExchangeRateDto
                    {
                        Currency = rate.Currency,
                        Quantity = rate.Quantity,
                        ExchangeDate = DateTime.Parse(rate.Date),
                        Rate = rate.Rate
                    }).ToList();
        }
    }
}
