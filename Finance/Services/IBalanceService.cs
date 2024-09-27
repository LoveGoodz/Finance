using Finance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.Services
{
    public interface IBalanceService
    {
        Task<Balance> GetBalanceByIdAsync(int id);
        Task<bool> UpdateBalanceAsync(Balance balance);
        Task<Balance> AddBalanceAsync(Balance balance);
        Task<bool> DeleteBalanceAsync(int id);
        Task<(int TotalRecords, List<Balance> Data)> GetBalancesAsync(int? companyId, int? customerId, int pageNumber, int pageSize);
    }
}
