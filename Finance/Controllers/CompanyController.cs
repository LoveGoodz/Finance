using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly FinanceContext _context;

        public CompanyController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Company/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Company>>> GetAllCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompanyById(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound("Company kaydı bulunamadı.");
            }

            return company;
        }

        // PUT: api/Company/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.ID)
            {
                return BadRequest("ID parametresi ile Company.ID eşleşmiyor.");
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound("Company kaydı bulunamadı.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Company
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanyById), new { id = company.ID }, company);
        }

        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound("Company kaydı bulunamadı.");
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Company (Sayfalama ve Filtreleme)
        [HttpGet]
        public async Task<ActionResult> GetCompanies(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("PageNumber ve PageSize sıfırdan büyük olmalıdır.");
            }

            var query = _context.Companies.AsQueryable();

            // İsimle filtreleme
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var companies = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = companies });
        }

        // Veritabanında Company kaydı olup olmadığını kontrol eden yardımcı metot
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.ID == id);
        }
    }
}
