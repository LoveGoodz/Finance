using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class StockService : IStockService
    {
        private readonly FinanceContext _context;

        public StockService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<Stock> GetStockByIdAsync(int id)
        {
            return await _context.Stocks.FindAsync(id);
        }

        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<bool> UpdateStockAsync(int id, Stock stock)
        {
            if (id != stock.ID)
            {
                return false;
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return false;
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<Stock> Stocks, int TotalRecords)> GetStocksAsync(string name, int pageNumber, int pageSize)
        {
            var query = _context.Stocks.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            var totalRecords = await query.CountAsync();
            var stocks = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (stocks, totalRecords);
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.ID == id);
        }
    }
}
