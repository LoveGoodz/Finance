using Finance.Models;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransController : ControllerBase
    {
        private readonly IStockTransService _stockTransService;

        public StockTransController(IStockTransService stockTransService)
        {
            _stockTransService = stockTransService;
        }

        // Belirli bir stok işlemi getir
        [HttpGet("{id}")]
        public async Task<ActionResult<StockTrans>> GetStockTranById(int id)
        {
            var stockTran = await _stockTransService.GetStockTransByIdAsync(id);

            if (stockTran == null)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok işlemi başarıyla getirildi.", Data = stockTran });
        }

        // Stok işlemini güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockTran(int id, StockTrans stockTran)
        {
            var updated = await _stockTransService.UpdateStockTransAsync(id, stockTran);
            if (!updated)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok işlemi başarıyla güncellendi." });
        }

        // Yeni stok işlemi ekle
        [HttpPost]
        public async Task<ActionResult<StockTrans>> PostStockTran(StockTrans stockTran)
        {
            var createdStockTran = await _stockTransService.CreateStockTransAsync(stockTran);
            return CreatedAtAction(nameof(GetStockTranById), new { id = createdStockTran.ID }, new { Message = "Stok işlemi başarıyla eklendi.", Data = createdStockTran });
        }

        // Stok işlemini sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockTran(int id)
        {
            var deleted = await _stockTransService.DeleteStockTransAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok işlemi başarıyla silindi." });
        }

        // Stok işlemleri listele (sayfalama ve filtreleme ile)
        [HttpGet]
        public async Task<ActionResult> GetStockTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            var (stockTrans, totalRecords) = await _stockTransService.GetStockTransAsync(transactionType, pageNumber, pageSize);
            return Ok(new { TotalRecords = totalRecords, Data = stockTrans });
        }
    }
}
