using Application.Validators;
using Domain.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class ExchangeRatesService : IExchangeRatesService
{
    private readonly IExchangeRatesClient _client;
    private readonly DateValidator _validator;
    private readonly IExchangeRatesRepository _repository;
    public ExchangeRatesService(IExchangeRatesClient client, DateValidator validator, IExchangeRatesRepository repository)
    {
        _client = client;
        _validator = validator;
        _repository = repository;
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
    public async Task CleanupAsync()
    {
        await _repository.DeleteAll();
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

    private void ValidateDate(DateTime date)
    {
        var result = _validator.Validate(date);
        if (!result.IsValid)
        {
            throw new InvalidDateException(result.ToString());
        }
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
}
