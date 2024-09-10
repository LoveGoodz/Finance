using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Serilog; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly FinanceContext _context;

        public CustomerController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Customer/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            Log.Information("GetAllCustomers method called");

            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            Log.Information("GetCustomerById method called with id {Id}", id);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                Log.Warning("Customer with id {Id} not found", id);
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            return Ok(customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            Log.Information("PutCustomer method called with id {Id}", id);

            if (id != customer.ID)
            {
                Log.Error("ID parameter does not match Customer.ID");
                return BadRequest(new { Message = "ID parametresi ile Customer.ID eşleşmiyor.", Status = 400 });
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Log.Information("Customer with id {Id} updated successfully", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    Log.Warning("Customer with id {Id} not found", id);
                    return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
                }
                else
                {
                    Log.Error("An error occurred while updating customer with id {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            Log.Information("PostCustomer method called");

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            Log.Information("Customer with id {Id} created successfully", customer.ID);

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.ID }, customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            Log.Information("DeleteCustomer method called with id {Id}", id);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                Log.Warning("Customer with id {Id} not found", id);
                return NotFound(new { Message = "Müşteri kaydı bulunamadı.", Status = 404 });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            Log.Information("Customer with id {Id} deleted successfully", id);

            return NoContent();
        }

        // GET: api/Customer (Filtreleme ve Sayfalama)
        [HttpGet]
        public async Task<ActionResult> GetCustomers(string name = null, int pageNumber = 1, int pageSize = 10)
        {
            Log.Information("GetCustomers method called with parameters: name = {Name}, pageNumber = {PageNumber}, pageSize = {PageSize}", name, pageNumber, pageSize);

            if (pageNumber <= 0 || pageSize <= 0)
            {
                Log.Error("PageNumber and PageSize must be greater than zero");
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
