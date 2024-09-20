using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // Logger ekleniyor
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finance.Models;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly FinanceContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(FinanceContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Customer (Şirket bazlı müşteri listeleme)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(int? companyId)
        {
            IQueryable<Customer> query = _context.Customers.Include(c => c.Company);

            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyID == companyId);
                _logger.LogInformation("Şirket ID'sine göre müşteri listesi getiriliyor. CompanyID: {companyId}", companyId);
            }

            var customers = await query.ToListAsync();

            if (!customers.Any())
            {
                _logger.LogInformation("Listelenecek müşteri kaydı bulunamadı.");
                return NotFound(new { Message = "Müşteri kaydı bulunamadı." });
            }

            _logger.LogInformation("Tüm müşteriler getirildi.");
            return Ok(customers);
        }

        // POST: api/Customer (Yeni müşteri ekleme)
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO customerDTO)
        {
            _logger.LogInformation("Gelen müşteri verileri: {@CustomerDTO}", customerDTO);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (customerDTO.CompanyID == 0)
            {
                return BadRequest(new { Message = "Geçerli bir Şirket ID'si giriniz." });
            }

            var company = await _context.Companies.FindAsync(customerDTO.CompanyID);
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

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni müşteri eklendi. Müşteri ID: {Id}", customer.ID);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.ID }, customer);
        }

        // PUT: api/Customer/5 (Müşteri güncelleme)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerDTO customerDTO)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            customer.Name = customerDTO.Name;
            customer.Address = customerDTO.Address;
            customer.PhoneNumber = customerDTO.PhoneNumber;
            customer.Email = customerDTO.Email;
            customer.CompanyID = customerDTO.CompanyID;

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Müşteri güncellendi. ID: {Id}", customer.ID);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            return NoContent();
        }

        // GET: api/Customer/5 (Belirli müşteri bilgisi)
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.Include(c => c.Company).FirstOrDefaultAsync(c => c.ID == id);

            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            return Ok(customer);
        }

        // DELETE: api/Customer/5 (Müşteri silme)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
