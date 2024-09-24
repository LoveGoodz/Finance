using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ActTransController : ControllerBase
    {
        private readonly IDataAccessService _dataAccessService;

        public ActTransController(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = await _dataAccessService.GetAllAsync<ActTrans>();

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(at => at.TransactionType.Contains(transactionType)).ToList();
            }

            
            var pagedResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new { Data = pagedResult, TotalCount = query.Count });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActTranById(int id)
        {
            var actTran = await _dataAccessService.GetByIdAsync<ActTrans>(id);
            if (actTran == null) return NotFound();
            return Ok(actTran);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutActTran(int id, ActTrans actTran)
        {
            var existingActTran = await _dataAccessService.GetByIdAsync<ActTrans>(id);
            if (existingActTran == null) return NotFound();

            await _dataAccessService.UpdateAsync(actTran);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostActTran(ActTrans actTran)
        {
            await _dataAccessService.AddAsync(actTran);
            return CreatedAtAction(nameof(GetActTranById), new { id = actTran.ID }, actTran);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActTran(int id)
        {
            var actTran = await _dataAccessService.GetByIdAsync<ActTrans>(id);
            if (actTran == null) return NotFound();

            await _dataAccessService.DeleteAsync(actTran);
            return NoContent();
        }
    }
}
