using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize] // Bu controller'daki tüm action metotlarına JWT doğrulaması gerekiyor
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InvoiceController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Invoice/all - Tüm verileri listele, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            var invoices = await _context.Invoices.ToListAsync();
            return Ok(new { Message = "Tüm faturalar listelendi.", Data = invoices });
        }

        // GET: api/Invoice/5 - ID'ye göre Invoice getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura kaydı bulunamadı.", Status = 404 });
            }

            return Ok(invoice);
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

