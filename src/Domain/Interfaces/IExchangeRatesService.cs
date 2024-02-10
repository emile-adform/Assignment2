using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IExchangeRatesService
    {
        Task<List<CurrencyChangeDto>> GetCurrencyChanges(DateTime date);
        Task CleanupAsync();

    }
}
