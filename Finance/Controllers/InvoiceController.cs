using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InvoiceController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Invoice/{id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura bulunamadı." });
            }

            return Ok(invoice);
        }

        // POST: api/Invoice 
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz model verisi.", ModelState });
            }

            // Varsayılan olarak yeni faturalar taslak olarak eklenecek
            invoice.Status = "Taslak";
            invoice.CreatedAt = DateTime.UtcNow;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.ID }, invoice);
        }

        // PUT: api/Invoice/approve/{id} 
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveInvoice(int id)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.InvoiceDetails)
                                        .ThenInclude(d => d.Stock)
                                        .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura bulunamadı." });
            }

            if (invoice.Status != "Taslak")
            {
                return BadRequest(new { Message = "Sadece taslak durumundaki faturalar onaylanabilir." });
            }

            // Fişi onayla ve durumu güncelle
            invoice.Status = "Onaylandı";
            invoice.UpdatedAt = DateTime.UtcNow;

            // Stok hareketlerini oluştur
            foreach (var detail in invoice.InvoiceDetails)
            {
                var stockTrans = new StockTrans
                {
                    StockID = detail.StockID,
                    InvoiceDetailsID = detail.ID,
                    TransactionType = "Satış",
                    Quantity = detail.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.StockTrans.Add(stockTrans);
            }

            // Muhasebe hareketlerini oluştur
            var actTrans = new ActTrans
            {
                CustomerID = invoice.CustomerID,
                InvoiceID = invoice.ID,
                TransactionType = "Satış",
                Amount = invoice.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };
            _context.ActTrans.Add(actTrans);

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Fatura başarıyla onaylandı.", Invoice = invoice });
        }

        // PUT: api/Invoice/{id} 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile Invoice.ID eşleşmiyor." });
            }

            if (invoice.Status == "Onaylandı")
            {
                return BadRequest(new { Message = "Onaylanmış faturalar güncellenemez." });
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
                    return NotFound(new { Message = "Fatura bulunamadı." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Invoice/{id} 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura bulunamadı." });
            }

            if (invoice.Status == "Onaylandı")
            {
                return BadRequest(new { Message = "Onaylanmış faturalar silinemez." });
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Invoice 
        [HttpGet]
        public async Task<ActionResult> GetInvoices(string series = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "Sayfa numarası ve boyutu sıfırdan büyük olmalıdır." });
            }

            var query = _context.Invoices.AsQueryable();

            
            if (!string.IsNullOrEmpty(series))
            {
                query = query.Where(i => i.Series.Contains(series));
            }

            
            var totalRecords = await query.CountAsync();

            
            var invoices = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = invoices });
        }

        
        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.ID == id);
        }
    }
}
