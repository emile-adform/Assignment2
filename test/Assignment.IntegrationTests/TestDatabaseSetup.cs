using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.IntegrationTests;

public class TestDatabaseSetup
{
    private readonly IDbConnection _connection;
    public TestDatabaseSetup(IDbConnection connection)
    {
        _connection = connection;
    }
    public void SetupDatabase()
    {
        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS currency_rates (
                id serial PRIMARY KEY,
	            currency VARCHAR(3),
	            quantity int,
	            rate DECIMAL,
	            exchange_date DATE
	            );");
    }
    public IDbConnection GetConnection()
    {
        return _connection;
    }
}
