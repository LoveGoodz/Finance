using Finance.Models;

namespace Finance.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomersAsync(int? companyId);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> AddCustomerAsync(CustomerDTO customerDto);
        Task<bool> UpdateCustomerAsync(int id, CustomerDTO customerDto);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
