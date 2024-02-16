using Domain.Interfaces;
using MediatR;

namespace Application.ExchangeRates.Commands
{
    public class CleanupDatabaseCommand : IRequest { };

    public class CleanupDatabaseCommandHandler : IRequestHandler<CleanupDatabaseCommand>
    {
        private readonly IExchangeRatesRepository _repository;
        public CleanupDatabaseCommandHandler(IExchangeRatesRepository repository)
        {
            _repository = repository;
        }
        public async Task Handle(CleanupDatabaseCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAll();
        }
    }
}
