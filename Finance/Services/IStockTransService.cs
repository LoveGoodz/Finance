using Finance.Models;

namespace Finance.Services
{
    public interface IStockTransService
    {
        Task<StockTrans> GetStockTransByIdAsync(int id);
        Task<StockTrans> CreateStockTransAsync(StockTrans stockTrans);
        Task<bool> UpdateStockTransAsync(int id, StockTrans stockTrans);
        Task<bool> DeleteStockTransAsync(int id);
        Task<(IEnumerable<StockTrans> StockTrans, int TotalRecords)> GetStockTransAsync(string transactionType, int pageNumber, int pageSize);
    }
}
