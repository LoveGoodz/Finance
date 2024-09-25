using Finance.Models;

namespace Finance.Services
{
    public interface IInvoiceService
    {
        Task<Invoice> GetInvoiceByIdAsync(int id);
        Task<Invoice> CreateInvoiceAsync(InvoiceDTO invoiceDto);
        Task<bool> ApproveInvoiceAsync(int id);
        Task<bool> UpdateInvoiceAsync(int id, InvoiceDTO invoiceDto);
        Task<bool> DeleteInvoiceAsync(int id);
        Task<(IEnumerable<Invoice> Invoices, int TotalRecords)> GetInvoicesAsync(string series, int pageNumber, int pageSize);
    }
}
