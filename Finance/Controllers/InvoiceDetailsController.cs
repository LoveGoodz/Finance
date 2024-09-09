using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InvoiceDetailsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/InvoiceDetails/all - Tüm verileri döndürür, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<InvoiceDetails>>> GetAllInvoiceDetails()
        {
            return await _context.InvoiceDetails.ToListAsync();
        }

        // GET: api/InvoiceDetails/5 - ID'ye göre InvoiceDetail getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetails>> GetInvoiceDetailById(int id)
        {
            var invoiceDetail = await _context.InvoiceDetails.FindAsync(id);

            if (invoiceDetail == null)
            {
                return NotFound("Fatura detayı kaydı bulunamadı.");
            }

            return invoiceDetail;
        }

        // PUT: api/InvoiceDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceDetail(int id, InvoiceDetails invoiceDetail)
        {
            if (id != invoiceDetail.ID)
            {
                return BadRequest("ID parametresi ile InvoiceDetail.ID eşleşmiyor.");
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
                    return NotFound("Fatura detayı kaydı bulunamadı.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/InvoiceDetails
        [HttpPost]
        public async Task<ActionResult<InvoiceDetails>> PostInvoiceDetail(InvoiceDetails invoiceDetail)
        {
            _context.InvoiceDetails.Add(invoiceDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoiceDetailById), new { id = invoiceDetail.ID }, invoiceDetail);
        }

        // DELETE: api/InvoiceDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceDetail(int id)
        {
            var invoiceDetail = await _context.InvoiceDetails.FindAsync(id);
            if (invoiceDetail == null)
            {
                return NotFound("Fatura detayı kaydı bulunamadı.");
            }

            _context.InvoiceDetails.Remove(invoiceDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/InvoiceDetails (Filtreleme ve Sayfalama)
        [HttpGet]
        public async Task<ActionResult> GetInvoiceDetails(int? stockId = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("PageNumber ve PageSize sıfırdan büyük olmalıdır.");
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
