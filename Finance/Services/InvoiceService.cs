using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly FinanceContext _context;
        private readonly IStockService _stockService;

        public InvoiceService(FinanceContext context, IStockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Stock)
                .Include(i => i.Company)
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null) return null;

            return new InvoiceDTO
            {
                ID = invoice.ID,
                CustomerID = invoice.CustomerID,
                CompanyID = invoice.CompanyID,
                CustomerName = invoice.Customer?.Name ?? "Müşteri bilgisi eksik",
                CompanyName = invoice.Company?.Name ?? "Şirket bilgisi eksik",
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount,
                Series = invoice.Series,
                Status = invoice.Status,
                InvoiceDetails = invoice.InvoiceDetails.Select(detail => new InvoiceDetailsDTO
                {
                    InvoiceID = detail.InvoiceID,
                    StockID = detail.StockID,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.Quantity * detail.UnitPrice,
                    StockName = detail.Stock?.Name
                }).ToList()
            };
        }

        public async Task<InvoiceDTO> CreateInvoiceAsync(InvoiceDTO invoiceDto)
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

            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Series == invoice.Series && i.CustomerID == invoice.CustomerID);

            if (existingInvoice != null)
            {
                throw new InvalidOperationException("Bu seri ve müşteriyle zaten bir fatura mevcut.");
            }

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            invoiceDto.ID = invoice.ID;
            invoiceDto.CustomerName = (await _context.Customers.FindAsync(invoiceDto.CustomerID))?.Name ?? "Müşteri bilgisi eksik";
            invoiceDto.CompanyName = (await _context.Companies.FindAsync(invoiceDto.CompanyID))?.Name ?? "Şirket bilgisi eksik";

            return invoiceDto;
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int id, string newStatus)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null)
            {
                return false;
            }

            foreach (var detail in invoice.InvoiceDetails)
            {
                await _stockService.UpdateStockForInvoice(detail.StockID, detail.Quantity, newStatus);
            }

            invoice.Status = newStatus;
            invoice.UpdatedAt = DateTime.UtcNow;

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

            invoice.InvoiceDetails.Clear();
            foreach (var detail in invoiceDto.InvoiceDetails)
            {
                var invoiceDetail = new InvoiceDetails
                {
                    StockID = detail.StockID,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.Quantity * detail.UnitPrice,
                    InvoiceID = invoice.ID,
                    CreatedAt = DateTime.UtcNow
                };
                invoice.InvoiceDetails.Add(invoiceDetail);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (invoice == null || invoice.Status == "Onaylandı")
            {
                return false;
            }

            _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
            await _context.SaveChangesAsync();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(IEnumerable<InvoiceDTO> Invoices, int TotalRecords)> GetInvoicesAsync(string series, int pageNumber, int pageSize)
        {
            var query = _context.Invoices
                .Include(i => i.Company)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Stock)
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

            var invoiceDtos = invoices.Select(invoice => new InvoiceDTO
            {
                ID = invoice.ID,
                CustomerID = invoice.CustomerID,
                CompanyID = invoice.CompanyID,
                CustomerName = invoice.Customer?.Name ?? "Müşteri bilgisi eksik",
                CompanyName = invoice.Company?.Name ?? "Şirket bilgisi eksik",
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount,
                Series = invoice.Series,
                Status = invoice.Status,
                InvoiceDetails = invoice.InvoiceDetails.Select(detail => new InvoiceDetailsDTO
                {
                    InvoiceID = detail.InvoiceID,
                    StockID = detail.StockID,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.Quantity * detail.UnitPrice,
                    StockName = detail.Stock?.Name
                }).ToList()
            });

            return (invoiceDtos, totalRecords);
        }
    }
}
