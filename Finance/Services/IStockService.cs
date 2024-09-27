using Finance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.Services
{
    public interface IStockService
    {
        Task<IEnumerable<Stock>> GetStocksAsync(int? companyId);
        Task<Stock> GetStockByIdAsync(int id);
        Task<Stock> AddStockAsync(StockDTO stockDto);
        Task<bool> UpdateStockAsync(int id, StockDTO stockDto);
        Task<bool> DeleteStockAsync(int id);
        Task<bool> UpdateStockForInvoice(int stockID, int quantity, string status);
    }
}
