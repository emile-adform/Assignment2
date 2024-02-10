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
        /// <summary>
        /// Gets currency rates changes for selected date
        /// </summary>
        /// 
        /// <remarks>
        /// Dates only available up until 2014/12/31
        /// </remarks>
        /// <param name="date"></param>
        /// <returns> The list of changes for currency rates </returns>
        /// <response code="200">Request successfuly achieved</response>
        /// <response code="400">Invalid date</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Server error</response>
        [HttpGet]
        public async Task<IActionResult> GetCurrencyRates([FromQuery]DateTime date)
        {
            return Ok(await _service.GetCurrencyChanges(date));
        }
    }
}
