using Application.Services;
using Application.Validators;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection service)
    {
        service.AddTransient<IExchangeRatesService, ExchangeRatesService>();
        service.AddTransient<DateValidator>();

    }
}
