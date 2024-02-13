using Application.Services;
using Application.Validators;
using AutoFixture.Xunit2;
using Castle.Core.Configuration;
using Domain.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
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
        private readonly Mock<IExchangeRatesClient> _mockClient;
        private readonly DateValidator _validator;
        private readonly ExchangeRatesService _service;
        public ExchangeRatesServiceTests()
        {
            _mockRepository = new Mock<IExchangeRatesRepository>();
            _mockClient = new Mock<IExchangeRatesClient>();
            _validator = new DateValidator();
            _service = new ExchangeRatesService(_mockClient.Object, _validator, _mockRepository.Object);
        }

        [Fact]
        public void InvalidDate_throwsInvalidDateException()
        {
            // ARRANGE
            DateTime invalidDate = new DateTime(2023, 02, 02);

            // ACT AND ASSERT
            Action act = () => _service.Invoking(s => s.GetCurrencyChanges(invalidDate))
                         .Should().ThrowAsync<InvalidDateException>()
                         .WithMessage("The date cannot be after 2014/12/31");
        }

        [Theory]
        [AutoData]
        public async Task GetCurrencyChanges_GivenValidDate_WhenDataIsInDatabase_CallsRepositoryTwice(List<ExchangeRateEntity> entities)
        {
            DateTime validDate = new DateTime(2012, 02, 02);

            _mockRepository.Setup(x => x.GetExchangeRatesAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<ExchangeRateEntity>(entities));

            await _service.GetCurrencyChanges(validDate);

            _mockRepository.Verify(i => i.GetExchangeRatesAsync(validDate), Times.Once());
            _mockRepository.Verify(i => i.GetExchangeRatesAsync(validDate.AddDays(-1)), Times.Once());

        }
        [Fact]
        public async Task GetCurrencyChanges_GivenValidDate_WithoutDataIsInDatabase_CallsClientTwice_CallsRepositoryTwice()
        {
            // ARRANGE
            DateTime validDate = new DateTime(2012, 02, 02);

            var item = new ExchangeRateItem
            {
                Date = "2001.02.02",
                Rate = 1.2M,
                Quantity = 1
            };
            List<ExchangeRateItem> list = new List<ExchangeRateItem> { item};

            var entity = new ExchangeRates
            {
                Rates = list
            };

            _mockRepository.Setup(x => x.GetExchangeRatesAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<ExchangeRateEntity>());
            _mockClient.Setup(x => x.GetExchangeRatesByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(entity);

            // ACT
            await _service.GetCurrencyChanges(validDate);

            // ASSERT
            _mockClient.Verify(i => i.GetExchangeRatesByDateAsync(validDate), Times.Once());
            _mockClient.Verify(i => i.GetExchangeRatesByDateAsync(validDate.AddDays(-1)), Times.Once());
            _mockRepository.Verify(i => i.InsertExchangeRatesAsync(It.IsAny<List<ExchangeRateEntity>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CalculateCurrencyChanges_CalculatesChangesCorrectlyAsync()
        {
            // ARRANGE
            DateTime validDate = new DateTime(2012, 02, 02);

            var selectedDateRates = new List<ExchangeRateEntity>
        {
            new ExchangeRateEntity { Currency = "USD", Rate = 1.2m, Quantity = 1, ExchangeDate = validDate },
            new ExchangeRateEntity { Currency = "EUR", Rate = 0.8m, Quantity = 1, ExchangeDate = validDate }
        };

            var priorDayRates = new List<ExchangeRateEntity>
        {
            new ExchangeRateEntity { Currency = "USD", Rate = 1.1m, Quantity = 1, ExchangeDate = validDate.AddDays(-1) },
            new ExchangeRateEntity { Currency = "EUR", Rate = 0.9m, Quantity = 1, ExchangeDate = validDate.AddDays(-1) }
        };

            _mockRepository.Setup(x => x.GetExchangeRatesAsync(validDate)).ReturnsAsync(selectedDateRates.ToArray());
            _mockRepository.Setup(x => x.GetExchangeRatesAsync(validDate.AddDays(-1))).ReturnsAsync(priorDayRates.ToArray());

            // ACT
            var currencyChanges = await _service.GetCurrencyChanges(validDate);

            // ASSERT
            currencyChanges.Should().HaveCount(2);

            currencyChanges.Should().ContainEquivalentOf(new CurrencyChangeDto
            {
                Change = 9.090909090909090909090909090M,
                Currency = "USD",
                ExchangeDate = validDate
            });

            currencyChanges.Should().ContainEquivalentOf(new CurrencyChangeDto
            {
                Change = -11.111111111111111111111111110M,
                Currency = "EUR",
                ExchangeDate = validDate
            });
        }
    }

}