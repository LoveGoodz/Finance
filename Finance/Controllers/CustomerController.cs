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
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IDataAccessService dataAccessService, ILogger<CustomerController> logger)
        {
            _dataAccessService = dataAccessService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(int? companyId)
        {
            var customers = await _dataAccessService.GetAllAsync<Customer>();

            if (companyId.HasValue)
            {
                customers = customers.Where(c => c.CompanyID == companyId.Value).ToList();
            }

            if (!customers.Any())
            {
                _logger.LogInformation("Müşteri kaydı bulunamadı.");
                return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
            }

            _logger.LogInformation("Tüm müşteriler getirildi.");
            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
            var company = await _dataAccessService.GetByIdAsync<Company>(customerDTO.CompanyID);
            if (company == null)
            {
                return BadRequest(new { Message = "Geçerli bir Şirket ID'si giriniz." });
            }

            var customer = new Customer
            {
                Name = customerDTO.Name,
                Address = customerDTO.Address,
                PhoneNumber = customerDTO.PhoneNumber,
                Email = customerDTO.Email,
                CompanyID = customerDTO.CompanyID
            };

            await _dataAccessService.AddAsync(customer);

            _logger.LogInformation("Yeni müşteri eklendi. Müşteri ID: {Id}", customer.ID);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.ID }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDTO customerDTO)
        {
            var customer = await _dataAccessService.GetByIdAsync<Customer>(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
            }

            customer.Name = customerDTO.Name;
            customer.Address = customerDTO.Address;
            customer.PhoneNumber = customerDTO.PhoneNumber;
            customer.Email = customerDTO.Email;
            customer.CompanyID = customerDTO.CompanyID;

            await _dataAccessService.UpdateAsync(customer);

            _logger.LogInformation("Müşteri güncellendi. ID: {Id}", id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _dataAccessService.GetByIdAsync<Customer>(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
            }

            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _dataAccessService.GetByIdAsync<Customer>(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
            }

            await _dataAccessService.DeleteAsync(customer);

            _logger.LogInformation("Müşteri silindi. ID: {Id}", id);
            return NoContent();
        }
    }
}
