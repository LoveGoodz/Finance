using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 

namespace Finance.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class ActTransController : ControllerBase
    {
        private readonly FinanceContext _context;

        public ActTransController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/ActTrans/all - Tüm verileri sayfalama olmadan döndürür
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ActTrans>>> GetAllActTrans()
        {
            return await _context.ActTrans.ToListAsync();
        }

        // GET: api/ActTrans/5 - ID'ye göre ActTrans getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<ActTrans>> GetActTranById(int id)
        {
            var actTran = await _context.ActTrans.FindAsync(id);

            if (actTran == null)
            {
                return NotFound("ActTrans kaydı bulunamadı.");
            }

            return actTran;
        }

        // PUT: api/ActTrans/5 - Mevcut bir ActTrans günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActTran(int id, ActTrans actTran)
        {
            if (id != actTran.ID)
            {
                return BadRequest("ID parametresi ve ActTrans.ID eşleşmiyor.");
            }

            _context.Entry(actTran).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActTranExists(id))
                {
                    return NotFound("ActTrans kaydı bulunamadı.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ActTrans - Yeni ActTrans ekler
        [HttpPost]
        public async Task<ActionResult<ActTrans>> PostActTran(ActTrans actTran)
        {
            _context.ActTrans.Add(actTran);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActTranById), new { id = actTran.ID }, actTran);
        }

        // DELETE: api/ActTrans/5 - Belirli ID'ye sahip ActTrans siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActTran(int id)
        {
            var actTran = await _context.ActTrans.FindAsync(id);
            if (actTran == null)
            {
                return NotFound("ActTrans kaydı bulunamadı.");
            }

            _context.ActTrans.Remove(actTran);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/ActTrans - Filtreleme ve sayfalama ile ActTrans getirir
        [HttpGet]
        public async Task<ActionResult> GetActTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("PageNumber ve PageSize sıfırdan büyük olmalıdır.");
            }

            var query = _context.ActTrans.AsQueryable();

            // TransactionType ile filtreleme
            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(at => at.TransactionType.Contains(transactionType));
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var actTrans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = actTrans });
        }

        // Veritabanında ActTrans olup olmadığını kontrol eden yardımcı metot
        private bool ActTranExists(int id)
        {
            return _context.ActTrans.Any(e => e.ID == id);
        }
    }
}
