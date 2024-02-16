using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.ExchangeRates.Queries
{
    public record GetExchangeRatesQuery : IRequest<List<ExchangeRateEntity>>;

    public class GetExchangeRatesQueryHandler : IRequestHandler<GetExchangeRatesQuery, List<ExchangeRateEntity>>
    {
        private readonly IExchangeRatesRepository _repository;

        public GetExchangeRatesQueryHandler(IExchangeRatesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ExchangeRateEntity>> Handle(GetExchangeRatesQuery request, CancellationToken cancellationToken)
        {
            return (await _repository.GetAllAsync()).ToList();
        }
    }
}
