using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(IDataAccessService dataAccessService, ILogger<CompanyController> logger)
        {
            _dataAccessService = dataAccessService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Geçersiz sayfa numarası veya boyutu.");
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            // Şirketleri filtreleyip sayfalı şekilde getirme
            var companies = await _dataAccessService.GetPagedAsync<Company>(
                pageNumber,
                pageSize,
                c => string.IsNullOrEmpty(name) || c.Name.Contains(name)
            );

            if (!companies.Any())
            {
                _logger.LogWarning("Hiç şirket bulunamadı.");
                return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
            }

            _logger.LogInformation("Veriler getirildi.");
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompanyById(int id)
        {
            var company = await _dataAccessService.GetByIdAsync<Company>(id);

            if (company == null)
            {
                _logger.LogWarning("Şirket kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
            }

            _logger.LogInformation("Veri getirildi. Şirket ID: {Id}", id);
            return Ok(company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (id != company.ID)
            {
                _logger.LogWarning("ID parametresi ile Company.ID eşleşmiyor.");
                return BadRequest(new { Message = "ID parametresi ile Company.ID eşleşmiyor.", Status = 400 });
            }

            await _dataAccessService.UpdateAsync(company);
            _logger.LogInformation("Şirket güncellendi. ID: {Id}", id);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            await _dataAccessService.AddAsync(company);
            _logger.LogInformation("Yeni şirket eklendi. ID: {Id}", company.ID);
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.ID }, company);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _dataAccessService.GetByIdAsync<Company>(id);
            if (company == null)
            {
                _logger.LogWarning("Şirket kaydı bulunamadı. ID: {Id}", id);
                return NotFound(new { Message = "Şirket kaydı bulunamadı.", Status = 404 });
            }

            await _dataAccessService.DeleteAsync(company);
            _logger.LogInformation("Şirket silindi. ID: {Id}", id);
            return NoContent();
        }
    }
}
