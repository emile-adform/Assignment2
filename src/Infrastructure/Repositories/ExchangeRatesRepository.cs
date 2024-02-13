using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Data;

namespace Infrastructure.Repositories;

public class ExchangeRatesRepository : IExchangeRatesRepository
{
    private readonly IDbConnection _connection;
    public ExchangeRatesRepository(IDbConnection connection)
    {
        _connection = connection;
    }
    public async Task InsertExchangeRatesAsync(List<ExchangeRateEntity> rates)
    {
        string sql = @"
                    INSERT INTO currency_rates(
                        exchange_date, 
                        currency, 
                        quantity, 
                        rate)
                    VALUES (
                        @ExchangeDate, 
                        @Currency, 
                        @Quantity, 
                        @Rate)";
        await _connection.ExecuteAsync(sql, rates);
    }
    public async Task<IEnumerable<ExchangeRateEntity>> GetExchangeRatesAsync(DateTime date)
    {
        string sql = @"
                    SELECT currency AS Currency, 
                        quantity AS Quantity, 
                        rate AS Rate, 
                        exchange_date AS ExchangeDate
                    FROM currency_rates
                    WHERE exchange_date = @Date";
        return await _connection.QueryAsync<ExchangeRateEntity>(sql, new {Date = date});
    }
    public async Task DeleteAll()
    {
        string sql = @"DELETE FROM currency_rates";
        await _connection.ExecuteAsync(sql);
    }
}
