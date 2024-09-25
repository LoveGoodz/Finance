using Finance.Models;
using Finance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // Şirket ID'ye göre müşterileri filtrelemek için GET endpoint
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] int? companyId)
        {
            try
            {
                var customers = await _customerService.GetCustomersAsync(companyId);
                if (customers == null || !customers.Any())
                {
                    _logger.LogInformation("Müşteri kaydı bulunamadı.");
                    return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
                }

                _logger.LogInformation("Müşteriler başarıyla getirildi.");
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError("Müşteri verileri getirilemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Müşteri verileri alınamadı." });
            }
        }

        // Tekil müşteri bilgisi getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogInformation("Müşteri kaydı bulunamadı. ID: {Id}", id);
                    return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError("Müşteri getirilemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Müşteri alınamadı." });
            }
        }

        // Yeni müşteri ekleme
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
            try
            {
                var customer = await _customerService.AddCustomerAsync(customerDTO);
                if (customer == null)
                {
                    return BadRequest(new { Message = "Geçerli bir Şirket ID'si giriniz." });
                }

                _logger.LogInformation("Yeni müşteri eklendi. Müşteri ID: {Id}", customer.ID);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.ID }, customer);
            }
            catch (Exception ex)
            {
                _logger.LogError("Müşteri eklenemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Müşteri eklenemedi." });
            }
        }

        // Müşteri bilgilerini güncelleme
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDTO customerDTO)
        {
            try
            {
                var success = await _customerService.UpdateCustomerAsync(id, customerDTO);
                if (!success)
                {
                    return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
                }

                _logger.LogInformation("Müşteri güncellendi. ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Müşteri güncellenemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Müşteri güncellenemedi." });
            }
        }

        // Müşteri silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var success = await _customerService.DeleteCustomerAsync(id);
                if (!success)
                {
                    return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
                }

                _logger.LogInformation("Müşteri silindi. ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Müşteri silinemedi: {Error}", ex.Message);
                return StatusCode(500, new { Message = "Müşteri silinemedi." });
            }
        }
    }
}
