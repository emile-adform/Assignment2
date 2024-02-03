using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment2.WebApi.Controllers
{
    [ApiController]
    [Route("GetCurrencyRatesByDate")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly ExchangeRatesService _service;
        public ExchangeRatesController(ExchangeRatesService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrencyRates(DateTime date)
        {
            return Ok(await _service.GetRatesByDateAsync(date));
        }
    }
}
