using Finance.Models;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDTO>> GetInvoiceById(int id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura bulunamadı." });
            }

            return Ok(invoice);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDTO>> PostInvoice(InvoiceDTO invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz model verisi.", ModelState });
            }

            var createdInvoice = await _invoiceService.CreateInvoiceAsync(invoiceDto);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = createdInvoice.ID }, createdInvoice);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] StatusUpdateDto statusUpdate)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(statusUpdate.Status))
            {
                return BadRequest(new { Message = "Geçersiz durum verisi." });
            }

            var updated = await _invoiceService.UpdateInvoiceStatusAsync(id, statusUpdate.Status);
            if (!updated)
            {
                return BadRequest(new { Message = "Fatura durumu güncellenemedi." });
            }

            return Ok(new { Message = "Fatura durumu başarıyla güncellendi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, InvoiceDTO invoiceDto)
        {
            var updated = await _invoiceService.UpdateInvoiceAsync(id, invoiceDto);
            if (!updated)
            {
                return BadRequest(new { Message = "Fatura güncellenemedi." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var deleted = await _invoiceService.DeleteInvoiceAsync(id);
            if (!deleted)
            {
                return BadRequest(new { Message = "Fatura silinemedi." });
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetInvoices(string series = null, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _invoiceService.GetInvoicesAsync(series, pageNumber, pageSize);
            return Ok(new { TotalRecords = result.TotalRecords, Data = result.Invoices });
        }
    }

    public class StatusUpdateDto
    {
        public string Status { get; set; }
    }
}
