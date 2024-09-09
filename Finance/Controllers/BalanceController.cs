using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly FinanceContext _context;

        public BalanceController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Balance/all - Tüm verileri listeler, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Balance>>> GetAllBalances()
        {
            return await _context.Balances.ToListAsync();
        }

        // GET: api/Balance/5 - ID'ye göre Balance getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<Balance>> GetBalanceById(int id)
        {
            var balance = await _context.Balances.FindAsync(id);

            if (balance == null)
            {
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            return Ok(balance);
        }

        // PUT: api/Balance/5 - Mevcut bir Balance günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBalance(int id, Balance balance)
        {
            if (id != balance.ID)
            {
                return BadRequest(new { Message = "ID parametresi ve Balance.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(balance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BalanceExists(id))
                {
                    return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Balance - Yeni Balance ekler
        [HttpPost]
        public async Task<ActionResult<Balance>> PostBalance(Balance balance)
        {
            _context.Balances.Add(balance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBalanceById), new { id = balance.ID }, balance);
        }

        // DELETE: api/Balance/5 - Belirli ID'ye sahip Balance'ı siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalance(int id)
        {
            var balance = await _context.Balances.FindAsync(id);
            if (balance == null)
            {
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            _context.Balances.Remove(balance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Balance - Sayfalama ve filtreleme ile Balance getirir
        [HttpGet]
        public async Task<ActionResult> GetBalances(int? companyId = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.Balances.AsQueryable();

            // companyId'ye göre filtreleme
            if (companyId.HasValue)
            {
                query = query.Where(b => b.CompanyID == companyId);
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var balances = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = balances });
        }

        // Veritabanında Balance olup olmadığını kontrol eden yardımcı metot
        private bool BalanceExists(int id)
        {
            return _context.Balances.Any(e => e.ID == id);
        }
    }
}
