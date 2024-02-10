using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
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
    }
}
