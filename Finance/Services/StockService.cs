using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Services
{
    public class StockService : IStockService
    {
        private readonly FinanceContext _context;

        public StockService(FinanceContext context)
        {
            _context = context;
        }

        // Şirket ID'ye göre stokları getirir
        public async Task<IEnumerable<Stock>> GetStocksAsync(int? companyId)
        {
            IQueryable<Stock> query = _context.Stocks.Include(s => s.Company);

            if (companyId.HasValue)
            {
                query = query.Where(s => s.CompanyID == companyId);
            }

            return await query.ToListAsync();
        }

        // ID'ye göre tekil stok getirir
        public async Task<Stock> GetStockByIdAsync(int id)
        {
            return await _context.Stocks
                .Include(s => s.Company)
                .FirstOrDefaultAsync(s => s.ID == id);
        }

        // Yeni stok ekleme
        public async Task<Stock> AddStockAsync(StockDTO stockDto)
        {
            var company = await _context.Companies.FindAsync(stockDto.CompanyID);
            if (company == null)
            {
                return null;
            }

            var stock = new Stock
            {
                Name = stockDto.Name,
                Quantity = stockDto.Quantity,
                UnitPrice = (decimal)stockDto.UnitPrice,  // double -> decimal dönüştürüldü
                CompanyID = stockDto.CompanyID,
                CreatedAt = stockDto.CreatedAt,
                UpdatedAt = stockDto.UpdatedAt
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return stock;
        }

        // Stok bilgilerini güncelleme
        public async Task<bool> UpdateStockAsync(int id, StockDTO stockDto)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return false;
            }

            stock.Name = stockDto.Name;
            stock.Quantity = stockDto.Quantity;
            stock.UnitPrice = (decimal)stockDto.UnitPrice;  // double -> decimal dönüştürüldü
            stock.UpdatedAt = stockDto.UpdatedAt;

            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        // Stok silme
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
    }
}
