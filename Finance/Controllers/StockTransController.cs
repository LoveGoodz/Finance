using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransController : ControllerBase
    {
        private readonly FinanceContext _context;

        public StockTransController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/StockTrans/all - Tüm stok işlemlerini getirir
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<StockTrans>>> GetAllStockTrans()
        {
            var stockTrans = await _context.StockTrans.ToListAsync();
            return Ok(new { Message = "Tüm stok işlemleri başarıyla getirildi.", Data = stockTrans });
        }

        // GET: api/StockTrans/5 - ID'ye göre stok işlemi getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<StockTrans>> GetStockTranById(int id)
        {
            var stockTran = await _context.StockTrans.FindAsync(id);

            if (stockTran == null)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            return Ok(new { Message = "Stok işlemi başarıyla getirildi.", Data = stockTran });
        }

        // PUT: api/StockTrans/5 - Stok işlemini günceller
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockTran(int id, StockTrans stockTran)
        {
            if (id != stockTran.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile StockTrans.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(stockTran).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockTranExists(id))
                {
                    return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StockTrans - Yeni stok işlemi ekler
        [HttpPost]
        public async Task<ActionResult<StockTrans>> PostStockTran(StockTrans stockTran)
        {
            _context.StockTrans.Add(stockTran);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStockTranById), new { id = stockTran.ID }, new { Message = "Stok işlemi başarıyla eklendi.", Data = stockTran });
        }

        // DELETE: api/StockTrans/5 - Belirli ID'ye sahip stok işlemini siler
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockTran(int id)
        {
            var stockTran = await _context.StockTrans.FindAsync(id);
            if (stockTran == null)
            {
                return NotFound(new { Message = "Stok işlemi kaydı bulunamadı.", Status = 404 });
            }

            _context.StockTrans.Remove(stockTran);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/StockTrans - Filtreleme ve Sayfalama ile stok işlemlerini getirir
        [HttpGet]
        public async Task<ActionResult> GetStockTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.StockTrans.AsQueryable();

            // İşlem tipi ile filtreleme
            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(st => st.TransactionType.Contains(transactionType));
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var stockTrans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = stockTrans });
        }

        // Veritabanında stok işlemi olup olmadığını kontrol eden yardımcı metot
        private bool StockTranExists(int id)
        {
            return _context.StockTrans.Any(e => e.ID == id);
        }
    }
}
