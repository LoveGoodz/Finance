using Finance.Models;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceDetailsController : ControllerBase
    {
        private readonly IDataAccessService _dataAccessService;
        private readonly IInvoiceDetailsService _invoiceDetailsService;

        public InvoiceDetailsController(IDataAccessService dataAccessService, IInvoiceDetailsService invoiceDetailsService)
        {
            _dataAccessService = dataAccessService;
            _invoiceDetailsService = invoiceDetailsService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDetails>> GetInvoiceDetailsById(int id)
        {
            var invoiceDetails = await _dataAccessService.GetByIdAsync<InvoiceDetails>(id);
            if (invoiceDetails == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı." });
            }

            return Ok(invoiceDetails);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDetails>> PostInvoiceDetails(InvoiceDetailsDTO detailsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz model verisi.", ModelState });
            }

            var invoiceDetails = await _invoiceDetailsService.CreateInvoiceDetailsAsync(detailsDto);
            return CreatedAtAction(nameof(GetInvoiceDetailsById), new { id = invoiceDetails.ID }, invoiceDetails);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceDetails(int id, InvoiceDetailsDTO detailsDto)
        {
            var invoiceDetails = await _dataAccessService.GetByIdAsync<InvoiceDetails>(id);
            if (invoiceDetails == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı." });
            }

            var updated = await _invoiceDetailsService.UpdateInvoiceDetailsAsync(id, detailsDto);
            if (!updated)
            {
                return BadRequest(new { Message = "Fatura detayı güncellenemedi." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceDetails(int id)
        {
            var invoiceDetails = await _dataAccessService.GetByIdAsync<InvoiceDetails>(id);
            if (invoiceDetails == null)
            {
                return NotFound(new { Message = "Fatura detayı bulunamadı." });
            }

            var deleted = await _invoiceDetailsService.DeleteInvoiceDetailsAsync(id);
            if (!deleted)
            {
                return BadRequest(new { Message = "Fatura detayı silinemedi." });
            }

            return NoContent();
        }
    }
}
