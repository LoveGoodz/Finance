using Finance.Models;

namespace Finance.Services
{
    public interface IStockService
    {
        Task<Stock> GetStockByIdAsync(int id);
        Task<Stock> CreateStockAsync(Stock stock);
        Task<bool> UpdateStockAsync(int id, Stock stock);
        Task<bool> DeleteStockAsync(int id);
        Task<(IEnumerable<Stock> Stocks, int TotalRecords)> GetStocksAsync(string name, int pageNumber, int pageSize);
    }
}
