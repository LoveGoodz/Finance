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
                Log.Warning("Yetkilendirme başarısız: Kullanıcı doğrulanamadı."); // Loglama: Uyarı
                return Unauthorized("Kullanıcı doğrulanamadı.");
            }
            return Ok();
        }

        // GET: api/ActTrans/all - Tüm verileri döndürür, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<IActionResult> GetAllActTrans()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            Log.Information("Tüm ActTrans verileri isteniyor. Kullanıcı: {Username}", username); // Loglama: Bilgi

            var actTrans = await _context.ActTrans.ToListAsync();
            Log.Information("Toplam {Count} ActTrans kaydı alındı.", actTrans.Count); // Loglama: Bilgi

            return Ok(actTrans);
        }

        // GET: api/ActTrans/5 - ID'ye göre ActTrans getirir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActTranById(int id)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            Log.Information("ID ile ActTrans verisi isteniyor: {Id} - Kullanıcı: {Username}", id, username); // Loglama: Bilgi

            var actTran = await _context.ActTrans.FindAsync(id);
            if (actTran == null)
            {
                Log.Warning("ActTrans kaydı bulunamadı: {Id}", id); // Loglama: Uyarı
                return NotFound("ActTrans kaydı bulunamadı.");
            }

            return Ok(actTran);
        }

        // PUT: api/ActTrans/5 - Mevcut bir ActTrans günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActTran(int id, ActTrans actTran)
        {
            if (id != actTran.ID)
            {
                Log.Warning("Güncelleme hatası: ID parametresi ile ActTrans.ID eşleşmiyor. ID: {Id}", id); // Loglama: Uyarı
                return BadRequest("ID parametresi ve ActTrans.ID eşleşmiyor.");
            }

            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            _context.Entry(actTran).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Log.Information("ActTrans güncellendi. ID: {Id}", id); // Loglama: Bilgi
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ActTranExists(id))
                {
                    Log.Error("ActTrans kaydı bulunamadı. ID: {Id}", id); // Loglama: Hata
                    return NotFound("ActTrans kaydı bulunamadı.");
                }
                else
                {
                    Log.Error(ex, "ActTrans güncellenirken bir hata oluştu. ID: {Id}", id); // Loglama: Hata
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ActTrans - Yeni ActTrans ekler
        [HttpPost]
        public async Task<IActionResult> PostActTran(ActTrans actTran)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            _context.ActTrans.Add(actTran);
            await _context.SaveChangesAsync();

            Log.Information("Yeni ActTrans eklendi. ID: {Id}", actTran.ID); // Loglama: Bilgi

            return CreatedAtAction(nameof(GetActTranById), new { id = actTran.ID }, actTran);
        }

        // DELETE: api/ActTrans/5 - Belirli ID'ye sahip ActTrans siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActTran(int id)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            var actTran = await _context.ActTrans.FindAsync(id);
            if (actTran == null)
            {
                Log.Warning("ActTrans kaydı silinemedi. Kayıt bulunamadı. ID: {Id}", id); // Loglama: Uyarı
                return NotFound("ActTrans kaydı bulunamadı.");
            }

            _context.ActTrans.Remove(actTran);
            await _context.SaveChangesAsync();

            Log.Information("ActTrans silindi. ID: {Id}", id); // Loglama: Bilgi

            return NoContent();
        }

        // GET: api/ActTrans - Filtreleme ve sayfalama ile ActTrans getirir
        [HttpGet]
        public async Task<IActionResult> GetActTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                Log.Warning("Geçersiz sayfa boyutu veya numarası. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize); // Loglama: Uyarı
                return BadRequest("PageNumber ve PageSize sıfırdan büyük olmalıdır.");
            }

            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var unauthorizedResult = UnauthorizedIfNull(username);
            if (unauthorizedResult is UnauthorizedResult) return unauthorizedResult;

            var query = _context.ActTrans.AsQueryable();

            // TransactionType ile filtreleme
            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(at => at.TransactionType.Contains(transactionType));
                Log.Information("TransactionType ile filtreleme yapıldı: {TransactionType}", transactionType); // Loglama: Bilgi
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var actTrans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Log.Information("{Count} ActTrans kaydı getirildi. PageNumber: {PageNumber}, PageSize: {PageSize}", actTrans.Count, pageNumber, pageSize); // Loglama: Bilgi

            return Ok(new { TotalRecords = totalRecords, Data = actTrans });
        }

        // Veritabanında ActTrans olup olmadığını kontrol eden yardımcı metot
        private bool ActTranExists(int id)
        {
            return _context.ActTrans.Any(e => e.ID == id);
        }
    }
}
