using Finance.Models;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockService stockService, ILogger<StockController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        // Şirket ID'ye göre stokları getirme
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStocks([FromQuery] int? companyId)
        {
            try
            {
                var stocks = await _stockService.GetStocksAsync(companyId);
                if (stocks == null || !stocks.Any())
                {
                    _logger.LogInformation("Stok kaydı bulunamadı.");
                    return NotFound(new { Message = "Stok kaydı bulunamadı." });
                }

                _logger.LogInformation("Stoklar başarıyla getirildi.");
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Stok verileri getirilemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Stok verileri alınamadı." });
            }
        }

        // Tekil stok bilgisi getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(int id)
        {
            try
            {
                var stock = await _stockService.GetStockByIdAsync(id);
                if (stock == null)
                {
                    _logger.LogInformation("Stok kaydı bulunamadı. ID: {Id}", id);
                    return NotFound(new { Message = "Stok kaydı bulunamadı." });
                }

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError("Stok getirilemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Stok alınamadı." });
            }
        }

        // Yeni stok ekleme
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(StockDTO stockDto)
        {
            try
            {
                var stock = await _stockService.AddStockAsync(stockDto);
                if (stock == null)
                {
                    return BadRequest(new { Message = "Geçerli bir Şirket ID'si giriniz." });
                }

                _logger.LogInformation("Yeni stok eklendi. Stok ID: {Id}", stock.ID);
                return CreatedAtAction(nameof(GetStockById), new { id = stock.ID }, stock);
            }
            catch (Exception ex)
            {
                _logger.LogError("Stok eklenemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Stok eklenemedi." });
            }
        }

        // Stok bilgilerini güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, StockDTO stockDto)
        {
            try
            {
                var success = await _stockService.UpdateStockAsync(id, stockDto);
                if (!success)
                {
                    return NotFound(new { Message = "Stok kaydı bulunamadı." });
                }

                _logger.LogInformation("Stok güncellendi. ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Stok güncellenemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Stok güncellenemedi." });
            }
        }

        // Stok silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            try
            {
                var success = await _stockService.DeleteStockAsync(id);
                if (!success)
                {
                    return NotFound(new { Message = "Stok kaydı bulunamadı." });
                }

                _logger.LogInformation("Stok silindi. ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Stok silinemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Stok silinemedi." });
            }
        }
    }
}

