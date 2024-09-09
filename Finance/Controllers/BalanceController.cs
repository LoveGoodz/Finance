using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly FinanceContext _context;

        public BalanceController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Balance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Balance>>> GetBalances()
        {
            return await _context.Balances.ToListAsync();
        }

        // GET: api/Balance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Balance>> GetBalance(int id)
        {
            var balance = await _context.Balances.FindAsync(id);

            if (balance == null)
            {
                return NotFound();
            }

            return balance;
        }

        // PUT: api/Balance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBalance(int id, Balance balance)
        {
            if (id != balance.ID)
            {
                return BadRequest();
            }

            _context.Entry(balance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BalanceExists(id))
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

        // POST: api/Balance
        [HttpPost]
        public async Task<ActionResult<Balance>> PostBalance(Balance balance)
        {
            _context.Balances.Add(balance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBalance", new { id = balance.ID }, balance);
        }

        // DELETE: api/Balance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalance(int id)
        {
            var balance = await _context.Balances.FindAsync(id);
            if (balance == null)
            {
                return NotFound();
            }

            _context.Balances.Remove(balance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BalanceExists(int id)
        {
            return _context.Balances.Any(e => e.ID == id);
        }

        [HttpGet]
        public async Task<ActionResult> GetBalances(int? companyId = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Balances.AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(b => b.CompanyID == companyId);
            }

            var totalRecords = await query.CountAsync();
            var balances = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = balances });
        }

    }
}
