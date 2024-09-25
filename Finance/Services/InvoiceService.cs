using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly FinanceContext _context;

        public InvoiceService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.Company)
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.ID == id);
        }

        public async Task<Invoice> CreateInvoiceAsync(InvoiceDTO invoiceDto)
        {
            var invoice = new Invoice
            {
                CustomerID = invoiceDto.CustomerID,
                CompanyID = invoiceDto.CompanyID,
                InvoiceDate = invoiceDto.InvoiceDate,
                TotalAmount = invoiceDto.TotalAmount,
                Series = invoiceDto.Series,
                Status = "Taslak",
                CreatedAt = DateTime.UtcNow,
                InvoiceDetails = invoiceDto.InvoiceDetails.Select(detail => new InvoiceDetails
                {
                    StockID = detail.StockID,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.Quantity * detail.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<bool> ApproveInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Stock)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null || invoice.Status != "Taslak")
            {
                return false;
            }

            invoice.Status = "Onaylandı";
            invoice.UpdatedAt = DateTime.UtcNow;

            foreach (var detail in invoice.InvoiceDetails)
            {
                var stockTrans = new StockTrans
                {
                    StockID = detail.StockID,
                    InvoiceDetailsID = detail.ID,
                    TransactionType = "Satış",
                    Quantity = detail.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.StockTrans.Add(stockTrans);
            }

            var actTrans = new ActTrans
            {
                CustomerID = invoice.CustomerID,
                InvoiceID = invoice.ID,
                TransactionType = "Satış",
                Amount = invoice.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };
            _context.ActTrans.Add(actTrans);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateInvoiceAsync(int id, InvoiceDTO invoiceDto)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null || invoice.Status == "Onaylandı")
            {
                return false;
            }

            invoice.CustomerID = invoiceDto.CustomerID;
            invoice.CompanyID = invoiceDto.CompanyID;
            invoice.InvoiceDate = invoiceDto.InvoiceDate;
            invoice.TotalAmount = invoiceDto.TotalAmount;
            invoice.Series = invoiceDto.Series;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null || invoice.Status == "Onaylandı")
            {
                return false;
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<Invoice> Invoices, int TotalRecords)> GetInvoicesAsync(string series, int pageNumber, int pageSize)
        {
            var query = _context.Invoices
                .Include(i => i.Company)
                .Include(i => i.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(series))
            {
                query = query.Where(i => i.Series.Contains(series));
            }

            var totalRecords = await query.CountAsync();
            var invoices = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (invoices, totalRecords);
        }
    }
}
