using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize] // Bu controller'daki tüm action metotlarına JWT doğrulaması gerekiyor
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly FinanceContext _context;

        public CustomerController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Customer/all - Tüm verileri döndürür, sayfalama ve filtreleme olmadan
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        // GET: api/Customer/5 - ID'ye göre müşteri getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            return Ok(customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.ID)
            {
                return BadRequest(new { Message = "ID parametresi ile Customer.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.ID }, customer);
        }

        // DELETE: api/Customer/5
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

        // GET: api/Customer (Filtreleme ve Sayfalama)
        [HttpGet]
        public async Task<ActionResult> GetCustomers(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(new { Message = "PageNumber ve PageSize sıfırdan büyük olmalıdır.", Status = 400 });
            }

            var query = _context.Customers.AsQueryable();

            // İsimle filtreleme
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name));
            }

            // Toplam kayıt sayısı
            var totalRecords = await query.CountAsync();

            // Sayfalama işlemi
            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { TotalRecords = totalRecords, Data = customers });
        }

        // Veritabanında müşteri olup olmadığını kontrol eden yardımcı metot
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.ID == id);
        }
    }
}
