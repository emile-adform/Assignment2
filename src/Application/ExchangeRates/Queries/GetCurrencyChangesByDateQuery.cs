using Application.Validators;
using Domain.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.ExchangeRates.Queries
{
    public record GetCurrencyChangesByDateQuery : IRequest<List<CurrencyChangeDto>>
    {
        public DateTime Date { get; set; }
    }
    public class GetCurrencyChangesByDateQueryHandler : IRequestHandler<GetCurrencyChangesByDateQuery, List<CurrencyChangeDto>>
    {
        private readonly IExchangeRatesClient _client;
        private readonly GetCurrencyChangesByDateQueryValidator _validator;
        private readonly IExchangeRatesRepository _repository;
        public GetCurrencyChangesByDateQueryHandler(IExchangeRatesClient client, IExchangeRatesRepository repository, GetCurrencyChangesByDateQueryValidator validator)
        {
            _client = client;
            _repository = repository;
            _validator = validator;
        }

        public async Task<List<CurrencyChangeDto>> Handle(GetCurrencyChangesByDateQuery request, CancellationToken cancellationToken)
        {
            ValidateDate(request);

            var SelectedDateRates = await GetExchangeRatesByDate(request.Date);
            var PriorDayRates = await GetExchangeRatesByDate(request.Date.AddDays(-1));

            var currencyChanges = CalculateCurrencyChanges(SelectedDateRates, PriorDayRates);

            var orderedList = currencyChanges.OrderByDescending(c => c.Change).ToList();
            return orderedList;
        }
        private async Task<List<ExchangeRateEntity>> GetExchangeRatesByDate(DateTime date)
        {
            var result = (await _repository.GetExchangeRatesAsync(date)).ToList();

            if (!result.Any())
            {
                var resultFromClient = await _client.GetExchangeRatesByDateAsync(date) ?? throw new ExternalApiDataException("Unable to retrieve data from external API.");

                result = resultFromClient.Rates!.Select(r => new ExchangeRateEntity
                {
                    Currency = r.Currency,
                    Quantity = r.Quantity,
                    ExchangeDate = DateTime.Parse(r.Date),
                    Rate = r.Rate
                }).ToList();

                await _repository.InsertExchangeRatesAsync(result);
            }
            return result;
        }
        private List<CurrencyChangeDto> CalculateCurrencyChanges(List<ExchangeRateEntity> selectedDateRates, List<ExchangeRateEntity> priorDayRates)
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
        private void ValidateDate(GetCurrencyChangesByDateQuery request)
        {
            var result = _validator.Validate(request);
            if (!result.IsValid)
            {
                throw new InvalidDateException(result.ToString());
            }
        }
    }
}
