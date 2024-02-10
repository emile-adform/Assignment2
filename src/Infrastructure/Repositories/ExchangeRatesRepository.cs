using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ExchangeRatesRepository
    {
        private readonly IDbConnection _connection;
        public ExchangeRatesRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task InsertExchangeRatesAsync()
        {

        }
    }
}
