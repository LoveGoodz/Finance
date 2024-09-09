using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly FinanceContext _context;

        public StockController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Stock/all - Tüm stokları listeler, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAllStocks()
        {
            var stocks = await _context.Stocks.ToListAsync();
            return Ok(new { Message = "Tüm stoklar başarıyla getirildi.", Data = stocks });
        }

        // GET: api/Stock/5 - Belirli ID'ye göre stok getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok başarıyla getirildi.", Data = stock });
        }

        // PUT: api/Stock/5 - Mevcut stok kaydını günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, Stock stock)
        {
            if (id != stock.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile Stock.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Stock - Yeni stok kaydı ekler
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockById), new { id = stock.ID }, new { Message = "Stok başarıyla eklendi.", Data = stock });
        }

        // DELETE: api/Stock/5 - Belirli ID'ye sahip stok kaydını siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound(new { Message = "Stok kaydı bulunamadı.", Status = 404 });
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Stock - Filtreleme ve sayfalama ile stokları getirir
        [HttpGet]
        public async Task<ActionResult> GetStocks(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.Stocks.AsQueryable();

            // Stok ismi ile filtreleme
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var stocks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = stocks });
        }

        // Veritabanında stok kaydı olup olmadığını kontrol eden yardımcı metot
        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.ID == id);
        }
    }
}
