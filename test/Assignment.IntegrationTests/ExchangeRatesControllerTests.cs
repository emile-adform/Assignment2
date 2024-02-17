using Assignment2.WebApi.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;


namespace Assignment.IntegrationTests
{
    public class ExchangeRatesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ExchangeRatesControllerTests(CustomWebApplicationFactory<Program> factory)
        {            
            _factory = factory;
            _client = factory
                .WithWebHostBuilder(builder => builder.UseSetting("Authentication:TestScheme:Enabled", "true"))
                .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            _client.DefaultRequestHeaders.Add(AuthConstants.ApiKeyHeaderName, "letmein");
        }

        [Fact]
        public async Task Get_Endpoint_ShouldReturnResult()
        {
            var result = await _client.GetAsync("/rates");
            result.EnsureSuccessStatusCode();
        }
    }
}
