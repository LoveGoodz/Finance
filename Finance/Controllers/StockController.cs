using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly FinanceContext _context;
        private readonly ILogger<StockController> _logger;

        public StockController(FinanceContext context, ILogger<StockController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Stock/{id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(int id)
        {
            _logger.LogInformation($"Stok ID {id} ile isteniyor.");
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                _logger.LogWarning($"Stok ID {id} bulunamadı.");
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            _logger.LogInformation($"Stok ID {id} başarıyla getirildi.");
            return Ok(new { Message = "Stok başarıyla getirildi.", Data = stock });
        }

        // PUT: api/Stock/5 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, Stock stock)
        {
            if (id != stock.ID)
            {
                _logger.LogWarning($"Gelen ID {id} ile stok ID {stock.ID} eşleşmiyor.");
                return BadRequest(new { Message = "ID parametresi ile Stock.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Stok ID {id} başarıyla güncellendi.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    _logger.LogWarning($"Stok ID {id} bulunamadı.");
                    return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    _logger.LogError($"Stok ID {id} güncellenirken hata oluştu.");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Stock 
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(Stock stock)
        {
            _logger.LogInformation("Yeni stok ekleniyor.");
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Stok ID {stock.ID} başarıyla eklendi.");
            return CreatedAtAction(nameof(GetStockById), new { id = stock.ID }, new { Message = "Stok başarıyla eklendi.", Data = stock });
        }

        // DELETE: api/Stock/5 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            _logger.LogInformation($"Stok ID {id} siliniyor.");
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                _logger.LogWarning($"Stok ID {id} bulunamadı.");
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Stok ID {id} başarıyla silindi.");
            return NoContent();
        }

        // GET: api/Stock 
        [HttpGet]
        public async Task<ActionResult> GetStocks(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Geçersiz PageNumber veya PageSize.");
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            _logger.LogInformation($"Stoklar filtreleniyor: İsim: {name}, Sayfa: {pageNumber}, Sayfa Boyutu: {pageSize}");
            var query = _context.Stocks.AsQueryable();

            
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            
            var totalRecords = await query.CountAsync();

            
            var stocks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Stoklar başarıyla getirildi.");
            return Ok(new { TotalRecords = totalRecords, Data = stocks });
        }

        // Veritabanında stok kaydı olup olmadığını kontrol eden yardımcı metot
        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.ID == id);
        }
    }
}
