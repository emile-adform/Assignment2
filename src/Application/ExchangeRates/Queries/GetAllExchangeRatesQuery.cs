using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.ExchangeRates.Queries
{
    public record GetAllExchangeRatesQuery : IRequest<List<ExchangeRateEntity>>;

    public class GetAllExchangeRatesQueryHandler : IRequestHandler<GetAllExchangeRatesQuery, List<ExchangeRateEntity>>
    {
        private readonly IExchangeRatesRepository _repository;

        public GetAllExchangeRatesQueryHandler(IExchangeRatesRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ExchangeRateEntity>> Handle(GetAllExchangeRatesQuery request, CancellationToken cancellationToken)
        {
            return (await _repository.GetAllAsync()).ToList();
        }
    }
}
