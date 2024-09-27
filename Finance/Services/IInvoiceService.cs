using Finance.Models;

namespace Finance.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDTO> GetInvoiceByIdAsync(int id);
        Task<InvoiceDTO> CreateInvoiceAsync(InvoiceDTO invoiceDto);
        Task<bool> UpdateInvoiceStatusAsync(int id, string newStatus);
        Task<bool> UpdateInvoiceAsync(int id, InvoiceDTO invoiceDto);
        Task<bool> DeleteInvoiceAsync(int id);
        Task<(IEnumerable<InvoiceDTO> Invoices, int TotalRecords)> GetInvoicesAsync(string series, int pageNumber, int pageSize);
    }
}
