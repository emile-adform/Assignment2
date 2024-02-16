using Application.ExchangeRates.Commands;
using Application.ExchangeRates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Assignment2.WebApi.Controllers
{
    [ApiController]
    [Route("rates")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ExchangeRatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetAllExchangeRatesQuery()));
        }
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _mediator.Send(new CleanupDatabaseCommand());
            return Ok();
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
        [HttpGet("GetCurrencyChangesByDate")]
        public async Task<IActionResult> GetByDate([FromQuery]DateTime date)
        {
            return Ok(await _mediator.Send(new GetCurrencyChangesByDateQuery { Date = date }));
        } 
    }
}
