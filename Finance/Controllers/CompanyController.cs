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
            _logger = logger;
        }

        // Tüm şirketleri getir (sayfalama ile)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Geçersiz sayfa numarası veya boyutu.");
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var cacheKey = "all_companies";
            var cachedData = await _redisDb.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedCompanies = JsonSerializer.Deserialize<IEnumerable<Company>>(cachedData);
                _logger.LogInformation("Veriler cache'den çekildi.");
                return Ok(cachedCompanies);
            }

            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            var companies = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (companies == null || companies.Count == 0)
            {
                _logger.LogWarning("Hiç şirket bulunamadı.");
                return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
            }

            var serializedCompanies = JsonSerializer.Serialize(companies);
            await _redisDb.StringSetAsync(cacheKey, serializedCompanies, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Veritabanından veriler getirildi ve cache'e eklendi.");
            return Ok(companies);
        }

        // Belirli bir şirketi getir
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompanyById(int id)
        {
            var cacheKey = $"company_{id}";
            var cachedData = await _redisDb.StringGetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedCompany = JsonSerializer.Deserialize<Company>(cachedData);
                _logger.LogInformation("Veri cache'den getirildi. Şirket ID: {Id}", id);
                return Ok(cachedCompany);
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning("Şirket kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
            }

            var serializedCompany = JsonSerializer.Serialize(company);
            await _redisDb.StringSetAsync(cacheKey, serializedCompany, TimeSpan.FromMinutes(5));

            _logger.LogInformation("Veri veritabanından getirildi. Şirket ID: {Id}", id);
            return Ok(company);
        }

        // Şirket güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.ID)
            {
                _logger.LogWarning("ID parametresi ile Company.ID eşleşmiyor.");
                return BadRequest(new { Message = "ID parametresi ile Company.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _redisDb.KeyDeleteAsync($"company_{id}");
                await _redisDb.KeyDeleteAsync("all_companies");

                _logger.LogInformation("Şirket güncellendi. ID: {Id}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    _logger.LogWarning("Şirket kaydı bulunamadı. ID: {Id}", id);
                    return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Şirket ekle
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _logger.LogInformation("Gelen şirket bilgisi: {@Company}", company);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model doğrulama hatası: {ModelStateErrors}", ModelState);
                return BadRequest(ModelState);
            }

            company.CreatedAt = DateTime.Now;
            company.UpdatedAt = DateTime.Now;

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            await _redisDb.KeyDeleteAsync("all_companies");

            _logger.LogInformation("Yeni şirket eklendi. ID: {Id}", company.ID);
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.ID }, company);
        }

        // Şirket sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning("Şirket kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            await _redisDb.KeyDeleteAsync($"company_{id}");
            await _redisDb.KeyDeleteAsync("all_companies");

            _logger.LogInformation("Şirket silindi. ID: {Id}", id);
            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.ID == id);
        }
    }
}
