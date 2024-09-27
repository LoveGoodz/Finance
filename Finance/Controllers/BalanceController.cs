using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Balance>> GetBalanceById(int id)
        {
            Log.Information("Balance kaydı isteniyor. ID: {Id}", id);
            var balance = await _balanceService.GetBalanceByIdAsync(id);

            if (balance == null)
            {
                Log.Warning("Balance kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            return Ok(balance);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBalance(int id, Balance balance)
        {
            if (id != balance.ID)
            {
                Log.Warning("ID parametresi ile Balance.ID eşleşmiyor. ID: {Id}", id);
                return BadRequest(new { Message = "ID parametresi ile Balance.ID eşleşmiyor.", Status = 400 });
            }

            var existingBalance = await _balanceService.GetBalanceByIdAsync(id);
            if (existingBalance == null)
            {
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            await _balanceService.UpdateBalanceAsync(balance);
            Log.Information("Balance kaydı güncellendi. ID: {Id}", id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Balance>> PostBalance(Balance balance)
        {
            var createdBalance = await _balanceService.AddBalanceAsync(balance);
            Log.Information("Yeni Balance kaydı eklendi. ID: {Id}", createdBalance.ID);
            return CreatedAtAction(nameof(GetBalanceById), new { id = createdBalance.ID }, createdBalance);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalance(int id)
        {
            var balance = await _balanceService.GetBalanceByIdAsync(id);
            if (balance == null)
            {
                Log.Warning("Balance kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            await _balanceService.DeleteBalanceAsync(id);
            Log.Information("Balance kaydı silindi. ID: {Id}", id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetBalances(int? companyId = null, int? customerId = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                Log.Warning("Geçersiz sayfa boyutu veya numarası. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var (totalRecords, balances) = await _balanceService.GetBalancesAsync(companyId, customerId, pageNumber, pageSize);
            Log.Information("{Count} Balance kaydı getirildi. PageNumber: {PageNumber}, PageSize: {PageSize}", balances.Count, pageNumber, pageSize);
            return Ok(new { TotalRecords = totalRecords, Data = balances });
        }
    }
}
