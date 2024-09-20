using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Finance.Models;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InvoiceDetailsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/InvoiceDetails/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetails>> GetInvoiceDetailsById(int id)
        {
            var invoiceDetails = await _context.InvoiceDetails
                                               .Include(d => d.Invoice)
                                               .Include(d => d.Stock)
                                               .FirstOrDefaultAsync(d => d.ID == id);

            if (invoiceDetails == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı." });
            }

            return Ok(invoiceDetails);
        }

        // POST: api/InvoiceDetails
        [HttpPost]
        public async Task<ActionResult<InvoiceDetails>> PostInvoiceDetails(InvoiceDetailsDTO detailsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz model verisi.", ModelState });
            }

            var invoiceDetails = new InvoiceDetails
            {
                InvoiceID = detailsDto.InvoiceID,
                StockID = detailsDto.StockID,
                Quantity = detailsDto.Quantity,
                UnitPrice = detailsDto.UnitPrice,
                TotalPrice = detailsDto.Quantity * detailsDto.UnitPrice, // Toplam fiyat hesaplanır
                CreatedAt = DateTime.UtcNow
            };

            _context.InvoiceDetails.Add(invoiceDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoiceDetailsById), new { id = invoiceDetails.ID }, invoiceDetails);
        }

        // PUT: api/InvoiceDetails/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceDetails(int id, InvoiceDetailsDTO detailsDto)
        {
            var invoiceDetails = await _context.InvoiceDetails.FindAsync(id);

            if (invoiceDetails == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı." });
            }

            invoiceDetails.StockID = detailsDto.StockID;
            invoiceDetails.Quantity = detailsDto.Quantity;
            invoiceDetails.UnitPrice = detailsDto.UnitPrice;
            invoiceDetails.TotalPrice = detailsDto.Quantity * detailsDto.UnitPrice;
            invoiceDetails.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/InvoiceDetails/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceDetails(int id)
        {
            var invoiceDetails = await _context.InvoiceDetails.FindAsync(id);
            if (invoiceDetails == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı." });
            }

            _context.InvoiceDetails.Remove(invoiceDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
