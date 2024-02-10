using Application.Validators;
using Domain.DTOs;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class ExchangeRatesService
    {
        private readonly IExchangeRatesClient _client;
        private readonly DateValidator _validator;
        public ExchangeRatesService(IExchangeRatesClient client, DateValidator validator)
        {
            _client = client;
            _validator = validator;
        }

        public async Task<List<CurrencyChangeDto>> GetCurrencyChanges(DateTime date)
        {
            ValidateDate(date);
            
            var SelectedDateRates = await GetExchangeRatesByDate(date);
            var PriorDayRates = await GetExchangeRatesByDate(date.AddDays(-1));

            var currencyChanges = CalculateCurrencyChanges(SelectedDateRates, PriorDayRates);

            var orderedList = currencyChanges.OrderByDescending(c => c.Change).ToList();
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

        private void ValidateDate(DateTime date)
        {
            var result = _validator.Validate(date);
            if (!result.IsValid)
            {
                throw new InvalidDateException(string.Join(", ", result.Errors.Select(error => error.ErrorMessage)));
            }
        }
        private List<CurrencyChangeDto> CalculateCurrencyChanges(List<ExchangeRateDto> selectedDateRates, List<ExchangeRateDto> priorDayRates)
        {
            var currencyChanges = new List<CurrencyChangeDto>();

            foreach (var currencyRate in selectedDateRates)
            {
                var priorRateInfo = priorDayRates.First(r => r.Currency == currencyRate.Currency);

                decimal change = ((currencyRate.Rate / currencyRate.Quantity) - (priorRateInfo.Rate / priorRateInfo.Quantity))
                    / priorRateInfo.Rate * 100;

                currencyChanges.Add(new CurrencyChangeDto
                {
                    Change = change,
                    Currency = currencyRate.Currency,
                    ExchangeDate = currencyRate.ExchangeDate
                });
            }
            return currencyChanges;
        }
    }
}
