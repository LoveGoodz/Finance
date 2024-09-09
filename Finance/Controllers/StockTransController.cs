using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockTransController : ControllerBase
    {
        private readonly FinanceContext _context;

        public StockTransController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/StockTrans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockTrans>>> GetStockTrans()
        {
            return await _context.StockTrans.ToListAsync();
        }

        // GET: api/StockTrans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockTrans>> GetStockTran(int id)
        {
            var stockTran = await _context.StockTrans.FindAsync(id);

            if (stockTran == null)
            {
                return NotFound();
            }

            return stockTran;
        }

        // PUT: api/StockTrans/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockTran(int id, StockTrans stockTran)
        {
            if (id != stockTran.ID)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/StockTrans
        [HttpPost]
        public async Task<ActionResult<StockTrans>> PostStockTran(StockTrans stockTran)
        {
            _context.StockTrans.Add(stockTran);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStockTran", new { id = stockTran.ID }, stockTran);
        }

        // DELETE: api/StockTrans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStockTran(int id)
        {
            var stockTran = await _context.StockTrans.FindAsync(id);
            if (stockTran == null)
            {
                return NotFound();
            }

            _context.StockTrans.Remove(stockTran);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StockTranExists(int id)
        {
            return _context.StockTrans.Any(e => e.ID == id);
        }

        [HttpGet]
        public async Task<ActionResult> GetStockTrans(string transactionType = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.StockTrans.AsQueryable();

            if (!string.IsNullOrEmpty(transactionType))
            {
                query = query.Where(st => st.TransactionType.Contains(transactionType));
            }

            var totalRecords = await query.CountAsync();
            var stockTrans = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = stockTrans });
        }

    }
}
