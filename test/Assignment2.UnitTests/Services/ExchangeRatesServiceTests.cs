using Application.Services;
using Application.Validators;
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
            DateTime invalidDate = new DateTime(2023, 02, 02);

            Action act = () => _service.Invoking(s => s.GetCurrencyChanges(invalidDate))
                         .Should().ThrowAsync<InvalidDateException>()
                         .WithMessage("The date cannot be after 2014/12/31");
        }
        [Fact]
        public async Task ValidDate_CallsGetExchangeRatesByDateTwice()
        {
            DateTime validDate = new DateTime(2012, 02, 02);

            var serviceMock = new Mock<ExchangeRatesService> { CallBase = true };

            // Setup the mock to return some dummy data for GetExchangeRatesByDate
            serviceMock.Setup(x => x.GetExchangeRatesByDate(It.IsAny<DateTime>()))
                       .ReturnsAsync(new List<ExchangeRateEntity>());

            // Act
            await serviceMock.Object.GetCurrencyChanges(validDate);
        }

    }
}
