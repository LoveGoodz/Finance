using Finance.Models;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockService stockService, IDataAccessService dataAccessService, ILogger<StockController> logger)
        {
            _stockService = stockService;
            _dataAccessService = dataAccessService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(int id)
        {
            _logger.LogInformation($"Stok ID {id} ile isteniyor.");
            var stock = await _dataAccessService.GetByIdAsync<Stock>(id);

            if (stock == null)
            {
                _logger.LogWarning($"Stok ID {id} bulunamadı.");
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok başarıyla getirildi.", Data = stock });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, Stock stock)
        {
            var stockExists = await _dataAccessService.GetByIdAsync<Stock>(id);
            if (stockExists == null)
            {
                _logger.LogWarning($"Stok ID {id} güncellenemedi, çünkü kayıt bulunamadı.");
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            await _dataAccessService.UpdateAsync(stock);  
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(Stock stock)
        {
            _logger.LogInformation("Yeni stok ekleniyor.");
            await _dataAccessService.AddAsync(stock);
            return CreatedAtAction(nameof(GetStockById), new { id = stock.ID }, new { Message = "Stok başarıyla eklendi.", Data = stock });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _dataAccessService.GetByIdAsync<Stock>(id);
            if (stock == null)
            {
                _logger.LogWarning($"Stok ID {id} bulunamadı, silinemedi.");
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            await _dataAccessService.DeleteAsync(stock);  
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetStocks(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            var (stocks, totalRecords) = await _stockService.GetStocksAsync(name, pageNumber, pageSize);
            return Ok(new { TotalRecords = totalRecords, Data = stocks });
        }
    }
}
