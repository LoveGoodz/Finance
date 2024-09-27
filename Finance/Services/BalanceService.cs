using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly FinanceContext _context;

        public BalanceService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<Balance> GetBalanceByIdAsync(int id)
        {
            return await _context.Balances.FindAsync(id);
        }

        public async Task<bool> UpdateBalanceAsync(Balance balance)
        {
            _context.Entry(balance).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BalanceExistsAsync(balance.ID))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<Balance> AddBalanceAsync(Balance balance)
        {
            _context.Balances.Add(balance);
            await _context.SaveChangesAsync();
            return balance;
        }

        public async Task<bool> DeleteBalanceAsync(int id)
        {
            var balance = await _context.Balances.FindAsync(id);
            if (balance == null)
            {
                return false;
            }
            _context.Balances.Remove(balance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(int TotalRecords, List<Balance> Data)> GetBalancesAsync(int? companyId, int? customerId, int pageNumber, int pageSize)
        {
            var query = _context.Balances.AsQueryable();

            // Eğer CompanyID verilmişse, şirket balanslarını filtrele
            if (companyId.HasValue)
            {
                query = query.Where(b => b.CompanyID == companyId);
            }
            // Eğer CustomerID verilmişse, müşteri balanslarını filtrele
            else if (customerId.HasValue)
            {
                query = query.Where(b => b.CustomerID == customerId);
            }

            var totalRecords = await query.CountAsync();

            var balances = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Her balance kaydının güncellenmiş hesaplamalarını yap
            foreach (var balance in balances)
            {
                balance.TotalDebit = await _context.Invoices
                    .Where(i => i.CustomerID == balance.CustomerID && i.Status == "Onaylandı")
                    .SumAsync(i => i.TotalAmount);

                balance.TotalCredit = await _context.Invoices
                    .Where(i => i.CompanyID == balance.CompanyID && i.Status == "Onaylandı")
                    .SumAsync(i => i.TotalAmount);
            }

            return (totalRecords, balances);
        }

        private async Task<bool> BalanceExistsAsync(int id)
        {
            return await _context.Balances.AnyAsync(e => e.ID == id);
        }
    }
}
