using Castle.Core.Configuration;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2.UnitTests.Services
{
    public class ExchangeRatesServiceTests
    {
        private readonly Mock<IExchangeRatesRepository> _mockRepository;
        private readonly IConfiguration _configuration;
        private readonly ExchangeRatesServiceTests _service;
        public ExchangeRatesServiceTests()
        {
            _mockRepository = new Mock<IExchangeRatesRepository>();
            _service = new ExchangeRatesService()
        }

    }
}
