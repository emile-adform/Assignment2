using Application.Services;
using Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddTransient<ExchangeRatesService>();
            service.AddTransient<DateValidator>();

        }
    }
}
