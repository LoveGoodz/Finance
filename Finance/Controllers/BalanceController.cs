using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IDataAccessService _dataAccessService;

        public BalanceController(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Balance>> GetBalanceById(int id)
        {
            Log.Information("Balance kaydı isteniyor. ID: {Id}", id);
            var balance = await _dataAccessService.GetByIdAsync<Balance>(id);

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

            var existingBalance = await _dataAccessService.GetByIdAsync<Balance>(id);
            if (existingBalance == null)
            {
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            await _dataAccessService.UpdateAsync(balance);
            Log.Information("Balance kaydı güncellendi. ID: {Id}", id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Balance>> PostBalance(Balance balance)
        {
            await _dataAccessService.AddAsync(balance);
            Log.Information("Yeni Balance kaydı eklendi. ID: {Id}", balance.ID);
            return CreatedAtAction(nameof(GetBalanceById), new { id = balance.ID }, balance);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalance(int id)
        {
            var balance = await _dataAccessService.GetByIdAsync<Balance>(id);
            if (balance == null)
            {
                Log.Warning("Balance kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Balance kaydı bulunamadı.", Status = 404 });
            }

            await _dataAccessService.DeleteAsync(balance);
            Log.Information("Balance kaydı silindi. ID: {Id}", id);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetBalances(int? companyId = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                Log.Warning("Geçersiz sayfa boyutu veya numarası. PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var balances = await _dataAccessService.GetPagedAsync<Balance>(pageNumber, pageSize, b => !companyId.HasValue || b.CompanyID == companyId);
            var totalRecords = await _dataAccessService.CountAsync<Balance>(b => !companyId.HasValue || b.CompanyID == companyId);

            Log.Information("{Count} Balance kaydı getirildi. PageNumber: {PageNumber}, PageSize: {PageSize}", balances.Count, pageNumber, pageSize);
            return Ok(new { TotalRecords = totalRecords, Data = balances });
        }
    }
}
