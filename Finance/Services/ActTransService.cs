using Finance.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class ActTransService : IActTransService
    {
        private readonly FinanceContext _context;

        public ActTransService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActTrans>> GetActTrans(string transactionType, int pageNumber, int pageSize)
        {
            var query = _context.ActTrans.AsQueryable();
            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(at => at.TransactionType.Contains(transactionType));
            }
            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<ActTrans> GetActTranById(int id)
        {
            return await _context.ActTrans.FindAsync(id);
        }

        public async Task<bool> PutActTran(int id, ActTrans actTran)
        {
            if (id != actTran.ID) return false;
            _context.Entry(actTran).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ActTrans> PostActTran(ActTrans actTran)
        {
            _context.ActTrans.Add(actTran);
            await _context.SaveChangesAsync();
            return actTran;
        }

        public async Task<bool> DeleteActTran(int id)
        {
            var actTran = await _context.ActTrans.FindAsync(id);
            if (actTran == null) return false;
            _context.ActTrans.Remove(actTran);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool ActTranExists(int id)
        {
            return _context.ActTrans.Any(e => e.ID == id);
        }
    }
}
