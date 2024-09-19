using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Serilog;

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

        private IActionResult UnauthorizedIfNull(string username)
        {
            if (username == null)
            {
                Log.Warning("Yetkilendirme başarısız: Kullanıcı doğrulanamadı.");
                return Unauthorized("Kullanıcı doğrulanamadı.");
            }
            return Ok();
        }

        // GET: api/ActTrans 
        [HttpGet]
        public async Task<IActionResult> GetActTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                Log.Warning("Geçersiz sayfa boyutu veya numarası. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return BadRequest("PageNumber ve PageSize sıfırdan büyük olmalıdır.");
            }

            var query = _context.ActTrans.AsQueryable();

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(at => at.TransactionType.Contains(transactionType));
            }

            var totalRecords = await query.CountAsync();

            var actTrans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = actTrans });
        }

        // GET: api/ActTrans/5 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActTranById(int id)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            var actTran = await _context.ActTrans.FindAsync(id);
            if (actTran == null)
            {
                Log.Warning("ActTrans kaydı bulunamadı: {Id}", id);
                return NotFound("ActTrans kaydı bulunamadı.");
            }

            return Ok(actTran);
        }

        // PUT: api/ActTrans/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActTran(int id, ActTrans actTran)
        {
            if (id != actTran.ID)
            {
                Log.Warning("Güncelleme hatası: ID parametresi ile ActTrans.ID eşleşmiyor. ID: {Id}", id);
                return BadRequest("ID parametresi ve ActTrans.ID eşleşmiyor.");
            }

            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            _context.Entry(actTran).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
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

        // POST: api/ActTrans 
        [HttpPost]
        public async Task<IActionResult> PostActTran(ActTrans actTran)
        {
            _context.ActTrans.Add(actTran);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActTranById), new { id = actTran.ID }, actTran);
        }

        // DELETE: api/ActTrans/5 
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

        private bool ActTranExists(int id)
        {
            return _context.ActTrans.Any(e => e.ID == id);
        }
    }
}
