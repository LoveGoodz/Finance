using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Claims;
using Serilog; 

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly FinanceContext _context;
        private readonly IDatabase _redisDb;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(FinanceContext context, IConnectionMultiplexer redis, ILogger<CompanyController> logger)
        {
            _context = context;
            _redisDb = redis.GetDatabase();
            _logger = logger; // Logger'ı al
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Company>>> GetAllCompanies()
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null)
            {
                _logger.LogWarning("Unauthorized access attempt.");
                return Unauthorized(new { Message = "Kullanıcı doğrulanamadı.", Status = 401 });
            }

            string cacheKey = "all_companies";
            string cachedData = await _redisDb.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedCompanies = JsonSerializer.Deserialize<IEnumerable<Company>>(cachedData);
                return Ok(cachedCompanies);
            }

            var companies = await _context.Companies.ToListAsync();
            var serializedCompanies = JsonSerializer.Serialize(companies);
            await _redisDb.StringSetAsync(cacheKey, serializedCompanies, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Fetched all companies from the database.");

            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompanyById(int id)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (username == null)
            {
                _logger.LogWarning("Unauthorized access attempt.");
                return Unauthorized(new { Message = "Kullanıcı doğrulanamadı.", Status = 401 });
            }

            string cacheKey = $"company_{id}";
            string cachedData = await _redisDb.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedCompany = JsonSerializer.Deserialize<Company>(cachedData);
                return Ok(cachedCompany);
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning("Company with ID {Id} not found.", id);
                return NotFound(new { Message = "Company kaydı bulunamadı.", Status = 404 });
            }

            var serializedCompany = JsonSerializer.Serialize(company);
            await _redisDb.StringSetAsync(cacheKey, serializedCompany, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Fetched company with ID {Id} from the database.", id);

            return Ok(company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.ID)
            {
                _logger.LogWarning("Mismatch between ID parameter and Company.ID.");
                return BadRequest(new { Message = "ID parametresi ile Company.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _redisDb.KeyDeleteAsync($"company_{id}");
                await _redisDb.KeyDeleteAsync("all_companies");

                _logger.LogInformation("Updated company with ID {Id}.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    _logger.LogWarning("Company with ID {Id} not found during update.", id);
                    return NotFound(new { Message = "Company kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            await _redisDb.KeyDeleteAsync("all_companies");

            _logger.LogInformation("Added new company with ID {Id}.", company.ID);

            return CreatedAtAction(nameof(GetCompanyById), new { id = company.ID }, company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning("Company with ID {Id} not found during deletion.", id);
                return NotFound(new { Message = "Company kaydı bulunamadı.", Status = 404 });
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            await _redisDb.KeyDeleteAsync($"company_{id}");
            await _redisDb.KeyDeleteAsync("all_companies");

            _logger.LogInformation("Deleted company with ID {Id}.", id);

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetCompanies(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid page number or page size.");
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            var totalRecords = await query.CountAsync();

            var companies = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Fetched companies with pagination: PageNumber {PageNumber}, PageSize {PageSize}.", pageNumber, pageSize);

            return Ok(new { TotalRecords = totalRecords, Data = companies });
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.ID == id);
        }
    }
}

