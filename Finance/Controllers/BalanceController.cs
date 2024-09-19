using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Serilog;

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

        // GET: api/Balance/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Balance>> GetBalanceById(int id)
        {
            Log.Information("Balance kaydı isteniyor. ID: {Id}", id);
            var balance = await _context.Balances.FindAsync(id);

            if (balance == null)
            {
                Log.Warning("Balance kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            return Ok(balance);
        }

        // PUT: api/Balance/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBalance(int id, Balance balance)
        {
            if (id != balance.ID)
            {
                Log.Warning("ID parametresi ile Balance.ID eşleşmiyor. ID: {Id}", id);
                return BadRequest(new { Message = "ID parametresi ve Balance.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(balance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Log.Information("Balance kaydı güncellendi. ID: {Id}", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BalanceExists(id))
                {
                    Log.Error("Balance kaydı bulunamadı. ID: {Id}", id);
                    return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    Log.Error(ex, "Balance güncelleme sırasında bir hata oluştu. ID: {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Balance 
        [HttpPost]
        public async Task<ActionResult<Balance>> PostBalance(Balance balance)
        {
            _context.Balances.Add(balance);
            await _context.SaveChangesAsync();

            Log.Information("Yeni Balance kaydı eklendi. ID: {Id}", balance.ID);
            return CreatedAtAction(nameof(GetBalanceById), new { id = balance.ID }, balance);
        }

        // DELETE: api/Balance/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalance(int id)
        {
            Log.Information("Balance kaydı siliniyor. ID: {Id}", id);
            var balance = await _context.Balances.FindAsync(id);
            if (balance == null)
            {
                Log.Warning("Balance kaydı silinemedi. Kayıt bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            _context.Balances.Remove(balance);
            await _context.SaveChangesAsync();

            Log.Information("Balance kaydı silindi. ID: {Id}", id);
            return NoContent();
        }

        // GET: api/Balance 
        [HttpGet]
        public async Task<ActionResult> GetBalances(int? companyId = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                Log.Warning("Geçersiz sayfa boyutu veya numarası. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            Log.Information("Balance kayıtları sayfalama ve filtreleme ile getiriliyor. CompanyID: {CompanyID}", companyId);

            var query = _context.Balances.AsQueryable();

            
            if (companyId.HasValue)
            {
                query = query.Where(b => b.CompanyID == companyId);
            }

            
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var balances = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Log.Information("{Count} Balance kaydı getirildi. PageNumber: {PageNumber}, PageSize: {PageSize}", balances.Count, pageNumber, pageSize);
            return Ok(new { TotalRecords = totalRecords, Data = balances });
        }

        
        private bool BalanceExists(int id)
        {
            return _context.Balances.Any(e => e.ID == id);
        }
    }
}
