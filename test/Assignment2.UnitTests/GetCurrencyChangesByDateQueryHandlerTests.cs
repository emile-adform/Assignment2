using Application.ExchangeRates.Queries;
using Application.Validators;
using AutoFixture.Xunit2;
using Domain.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2.UnitTests;

public class GetCurrencyChangesByDateQueryHandlerTests
{
    private readonly Mock<IExchangeRatesRepository> _mockRepository;
    private readonly Mock<IExchangeRatesClient> _mockClient;
    private readonly GetCurrencyChangesByDateQueryValidator _validator;
    private readonly GetCurrencyChangesByDateQueryHandler _handler;
    public GetCurrencyChangesByDateQueryHandlerTests()
    {
        _mockRepository = new Mock<IExchangeRatesRepository>();
        _mockClient = new Mock<IExchangeRatesClient>();
        _validator = new GetCurrencyChangesByDateQueryValidator();
        _handler = new GetCurrencyChangesByDateQueryHandler(_mockClient.Object, _mockRepository.Object, _validator);
    }
    [Fact]
    public async Task InvalidDate_throwsInvalidDateException()
    {
        // ARRANGE
        DateTime invalidDate = new DateTime(2015, 02, 02);

        // ACT
        Func<Task> act = async () => await _handler.Handle(new GetCurrencyChangesByDateQuery { Date = invalidDate }, CancellationToken.None);

        // ASSERT
        await act.Should().ThrowAsync<InvalidDateException>().WithMessage("The date cannot be after 2014/12/31");
    }

    [Theory]
    [AutoData]
    public async Task GetCurrencyChanges_WhenDataIsInDatabase_CallsRepositoryTwice(List<ExchangeRateEntity> entities)
    {
        DateTime validDate = new DateTime(2013, 02, 02);

        _mockRepository.Setup(x => x.GetExchangeRatesAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<ExchangeRateEntity>(entities));

        await _handler.Handle(new GetCurrencyChangesByDateQuery { Date = validDate }, CancellationToken.None);

        _mockRepository.Verify(i => i.GetExchangeRatesAsync(validDate), Times.Once());
        _mockRepository.Verify(i => i.GetExchangeRatesAsync(validDate.AddDays(-1)), Times.Once());
    }
    [Fact]
    public async Task GetCurrencyChanges_WithoutDataIsInDatabase_CallsCorrectDependencies()
    {
        // ARRANGE
        DateTime validDate = new DateTime(2012, 02, 02);

        var item = new ExchangeRateItem
        {
            Date = "2001.02.02",
            Rate = 1.2M,
            Quantity = 1
        };
        List<ExchangeRateItem> list = new List<ExchangeRateItem> { item };

        var entity = new ExchangeRates
        {
            Rates = list
        };

        _mockRepository.Setup(x => x.GetExchangeRatesAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<ExchangeRateEntity>());
        _mockClient.Setup(x => x.GetExchangeRatesByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(entity);

        // ACT
        await _handler.Handle(new GetCurrencyChangesByDateQuery { Date = validDate }, CancellationToken.None);

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
        var currencyChanges = await _handler.Handle(new GetCurrencyChangesByDateQuery { Date = validDate }, CancellationToken.None);

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
