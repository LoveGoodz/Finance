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
        private readonly IDataAccessService _dataAccessService;
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IDataAccessService dataAccessService, IInvoiceService invoiceService)
        {
            _dataAccessService = dataAccessService;
            _invoiceService = invoiceService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            var invoice = await _dataAccessService.GetByIdAsync<Invoice>(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura bulunamadı." });
            }

            return Ok(invoice);
        }

        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(InvoiceDTO invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz model verisi.", ModelState });
            }

            var invoice = await _invoiceService.CreateInvoiceAsync(invoiceDto);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.ID }, invoice);
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveInvoice(int id)
        {
            var approved = await _invoiceService.ApproveInvoiceAsync(id);
            if (!approved)
            {
                return BadRequest(new { Message = "Fatura onaylanamadı." });
            }

            return Ok(new { Message = "Fatura başarıyla onaylandı." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, InvoiceDTO invoiceDto)
        {
            var invoice = await _dataAccessService.GetByIdAsync<Invoice>(id);
            if (invoice == null)
            {
                return NotFound(new { Message = "Fatura bulunamadı." });
            }

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
}
