using Finance.Models;

namespace Finance.Services
{
    public interface IInvoiceDetailsService
    {
        Task<InvoiceDetails> GetInvoiceDetailsByIdAsync(int id);
        Task<InvoiceDetails> CreateInvoiceDetailsAsync(InvoiceDetailsDTO detailsDto);
        Task<bool> UpdateInvoiceDetailsAsync(int id, InvoiceDetailsDTO detailsDto);
        Task<bool> DeleteInvoiceDetailsAsync(int id);
    }
}
