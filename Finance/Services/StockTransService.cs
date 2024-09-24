using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class StockTransService : IStockTransService
    {
        private readonly FinanceContext _context;

        public StockTransService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<StockTrans> GetStockTransByIdAsync(int id)
        {
            return await _context.StockTrans.FindAsync(id);
        }

        public async Task<StockTrans> CreateStockTransAsync(StockTrans stockTrans)
        {
            _context.StockTrans.Add(stockTrans);
            await _context.SaveChangesAsync();
            return stockTrans;
        }

        public async Task<bool> UpdateStockTransAsync(int id, StockTrans stockTrans)
        {
            if (id != stockTrans.ID)
            {
                return false;
            }

            _context.Entry(stockTrans).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockTransExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteStockTransAsync(int id)
        {
            var stockTrans = await _context.StockTrans.FindAsync(id);
            if (stockTrans == null)
            {
                return false;
            }

            _context.StockTrans.Remove(stockTrans);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<StockTrans> StockTrans, int TotalRecords)> GetStockTransAsync(string transactionType, int pageNumber, int pageSize)
        {
            var query = _context.StockTrans.AsQueryable();

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(st => st.TransactionType.Contains(transactionType));
            }

            var totalRecords = await query.CountAsync();
            var stockTrans = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (stockTrans, totalRecords);
        }

        private bool StockTransExists(int id)
        {
            return _context.StockTrans.Any(e => e.ID == id);
        }
    }
}
