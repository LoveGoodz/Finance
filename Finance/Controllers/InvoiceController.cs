using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize] // JWT doğrulaması
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

        // POST: api/Invoice - Yeni Invoice ekler (Fiş Taslağı)
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            invoice.Status = "Taslak";  // Fiş başlangıçta "Taslak" olacak
            invoice.CreatedAt = DateTime.Now;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.ID }, invoice);
        }

        // PUT: api/Invoice/approve/5 - Fiş Onaylama İşlemi ve StockTrans, ActTrans İşlemleri
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveInvoice(int id)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.InvoiceDetails)
                                        .ThenInclude(d => d.Stock)
                                        .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null)
                return NotFound(new { Message = "Fatura kaydı bulunamadı.", Status = 404 });

            if (invoice.Status != "Taslak")
                return BadRequest(new { Message = "Sadece taslak durumundaki faturalar onaylanabilir.", Status = 400 });

            // Fişi onayla ve durumu "Onaylandı" olarak güncelle
            invoice.Status = "Onaylandı";
            invoice.UpdatedAt = DateTime.Now;

            // Stok hareketlerini oluştur
            foreach (var detail in invoice.InvoiceDetails)
            {
                var stockTrans = new StockTrans
                {
                    StockID = detail.StockID,
                    InvoiceDetailsID = detail.ID,
                    TransactionType = "Satış", // ya da "Alış", duruma göre değiştirilebilir
                    Quantity = detail.Quantity,
                    CreatedAt = DateTime.Now
                };
                _context.StockTrans.Add(stockTrans);
            }

            // Muhasebe hareketlerini oluştur
            var actTrans = new ActTrans
            {
                CustomerID = invoice.CustomerID,
                InvoiceID = invoice.ID,
                TransactionType = "Satış", // ya da "Alış", duruma göre
                Amount = invoice.TotalAmount,
                CreatedAt = DateTime.Now
            };
            _context.ActTrans.Add(actTrans);

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Fatura başarıyla onaylandı.", Invoice = invoice });
        }

        // PUT: api/Invoice/5 - Mevcut bir Invoice günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile Invoice.ID eşleşmiyor.", Status = 400 });
            }

            if (invoice.Status == "Onaylandı")
            {
                return BadRequest(new { Message = "Onaylanmış faturalar güncellenemez.", Status = 400 });
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

        // DELETE: api/Invoice/5 - Belirli ID'ye sahip Invoice siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura kaydı bulunamadı.", Status = 404 });
            }

            if (invoice.Status == "Onaylandı")
            {
                return BadRequest(new { Message = "Onaylanmış faturalar silinemez.", Status = 400 });
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
