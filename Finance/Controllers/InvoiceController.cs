using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Finance.Controllers
{
    [Authorize] // Bu controller'daki tüm action metotlarına JWT doğrulaması gerekiyor
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly FinanceContext _context;
        private readonly IDistributedCache _cache;

        public InvoiceController(FinanceContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/Invoice/all - Tüm verileri listele, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            var cacheKey = "all_invoices";
            var cachedInvoices = await _cache.GetStringAsync(cacheKey);

            if (cachedInvoices != null)
            {
                var invoices = JsonSerializer.Deserialize<IEnumerable<Invoice>>(cachedInvoices);
                return Ok(new { Message = "Tüm faturalar cache'den listelendi.", Data = invoices });
            }

            var invoicesFromDb = await _context.Invoices.ToListAsync();
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(invoicesFromDb), options);

            return Ok(new { Message = "Tüm faturalar veritabanından listelendi.", Data = invoicesFromDb });
        }

        // GET: api/Invoice/5 - ID'ye göre Invoice getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            var cacheKey = $"invoice_{id}";
            var cachedInvoice = await _cache.GetStringAsync(cacheKey);

            if (cachedInvoice != null)
            {
                var invoice = JsonSerializer.Deserialize<Invoice>(cachedInvoice);
                return Ok(invoice);
            }

            var invoiceFromDb = await _context.Invoices.FindAsync(id);

            if (invoiceFromDb == null)
            {
                return NotFound(new { Message = "Fatura kaydı bulunamadı.", Status = 404 });
            }

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(invoiceFromDb), options);

            return Ok(invoiceFromDb);
        }

        // PUT: api/Invoice/5 - Mevcut bir Invoice günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile Invoice.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync($"invoice_{id}");
                // Remove the cache entry for the "all" invoices
                await _cache.RemoveAsync("all_invoices");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound(new { Message = "Fatura kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Invoice - Yeni Invoice ekler
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync("all_invoices");

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.ID }, invoice);
        }

        // DELETE: api/Invoice/5 - Belirli ID'ye sahip Invoice siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura kaydı bulunamadı.", Status = 404 });
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync($"invoice_{id}");
            await _cache.RemoveAsync("all_invoices");

            return NoContent();
        }

        // GET: api/Invoice - Filtreleme ve Sayfalama
        [HttpGet]
        public async Task<ActionResult> GetInvoices(string series = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.Invoices.AsQueryable();

            // Fatura serisine göre filtreleme
            if (!string.IsNullOrEmpty(series))
            {
                query = query.Where(i => i.Series.Contains(series));
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var invoices = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = invoices });
        }

        // Veritabanında Invoice kaydı olup olmadığını kontrol eden yardımcı metot
        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.ID == id);
        }
    }
}
