using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize] // Tüm action metotları için JWT doğrulaması gerektiriyor
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InvoiceDetailsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/InvoiceDetails/all - Tüm InvoiceDetails verilerini döndürür
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<InvoiceDetails>>> GetAllInvoiceDetails()
        {
            var invoiceDetails = await _context.InvoiceDetails.ToListAsync();
            return Ok(new { Message = "Tüm fatura detayları başarıyla getirildi.", Data = invoiceDetails });
        }

        // GET: api/InvoiceDetails/5 - Belirli ID'ye göre InvoiceDetail getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetails>> GetInvoiceDetailById(int id)
        {
            var invoiceDetail = await _context.InvoiceDetails.FindAsync(id);

            if (invoiceDetail == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Fatura detayı başarıyla getirildi.", Data = invoiceDetail });
        }

        // PUT: api/InvoiceDetails/5 - Mevcut InvoiceDetail kaydını günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceDetail(int id, InvoiceDetails invoiceDetail)
        {
            if (id != invoiceDetail.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile InvoiceDetail.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(invoiceDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceDetailExists(id))
                {
                    return NotFound(new { Message = "Fatura detayı kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Güncelleme hatası oluştu.", Status = 500 });
                }
            }

            return NoContent();
        }

        // POST: api/InvoiceDetails - Yeni InvoiceDetail kaydı ekler
        [HttpPost]
        public async Task<ActionResult<InvoiceDetails>> PostInvoiceDetail(InvoiceDetails invoiceDetail)
        {
            _context.InvoiceDetails.Add(invoiceDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoiceDetailById), new { id = invoiceDetail.ID }, new { Message = "Fatura detayı başarıyla eklendi.", Data = invoiceDetail });
        }

        // DELETE: api/InvoiceDetails/5 - Belirli ID'ye sahip InvoiceDetail kaydını siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceDetail(int id)
        {
            var invoiceDetail = await _context.InvoiceDetails.FindAsync(id);
            if (invoiceDetail == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı.", Status = 404 });
            }

            _context.InvoiceDetails.Remove(invoiceDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/InvoiceDetails - Filtreleme ve sayfalama ile InvoiceDetail verilerini getirir
        [HttpGet]
        public async Task<ActionResult> GetInvoiceDetails(int? stockId = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.InvoiceDetails.AsQueryable();

            // StockID ile filtreleme
            if (stockId.HasValue)
            {
                query = query.Where(id => id.StockID == stockId);
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var invoiceDetails = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = invoiceDetails });
        }

        // Veritabanında InvoiceDetail kaydı olup olmadığını kontrol eden yardımcı metot
        private bool InvoiceDetailExists(int id)
        {
            return _context.InvoiceDetails.Any(e => e.ID == id);
        }
    }
}
