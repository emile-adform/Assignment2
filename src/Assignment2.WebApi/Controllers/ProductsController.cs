using Application.ExchangeRates.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Assignment2.WebApi.Controllers
{
    [ApiController]
    [Route("exchanges")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new GetExchangeRatesQuery()));
        }
    }
}
