using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly FinanceContext _context;

        public CustomerService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync(int? companyId)
        {
            IQueryable<Customer> query = _context.Customers.Include(c => c.Company);

            if (companyId.HasValue)
            {
                query = query.Where(c => c.CompanyID == companyId);
            }

            return await query.ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<Customer> AddCustomerAsync(CustomerDTO customerDto)
        {
            var company = await _context.Companies.FindAsync(customerDto.CompanyID);
            if (company == null)
            {
                return null;
            }

            var customer = new Customer
            {
                Name = customerDto.Name,
                Address = customerDto.Address,
                PhoneNumber = customerDto.PhoneNumber,
                Email = customerDto.Email,
                CompanyID = customerDto.CompanyID
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerDTO customerDto)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }

            customer.Name = customerDto.Name;
            customer.Address = customerDto.Address;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.Email = customerDto.Email;
            customer.CompanyID = customerDto.CompanyID;

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
