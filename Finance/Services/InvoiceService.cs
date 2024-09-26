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

        public async Task<InvoiceDTO> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
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
                    TotalPrice = detail.Quantity * detail.UnitPrice // Hesaplama buraya taşındı
                }).ToList()
            };
        }

        public async Task<InvoiceDTO> CreateInvoiceAsync(InvoiceDTO invoiceDto)
        {
            // Yeni fatura nesnesini oluştur
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
                    TotalPrice = detail.Quantity * detail.UnitPrice, // TotalPrice hesaplanıyor
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            // Aynı fatura birden fazla kez eklenmemesi için kontrol (isteğe bağlı)
            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Series == invoice.Series && i.CustomerID == invoice.CustomerID);

            if (existingInvoice != null)
            {
                throw new InvalidOperationException("Bu seri ve müşteriyle zaten bir fatura mevcut.");
            }

            // Faturayı veritabanına ekle
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // DTO'nun yeni özelliklerini doldurun
            invoiceDto.ID = invoice.ID;
            invoiceDto.CustomerName = (await _context.Customers.FindAsync(invoiceDto.CustomerID))?.Name ?? "Müşteri bilgisi eksik";
            invoiceDto.CompanyName = (await _context.Companies.FindAsync(invoiceDto.CompanyID))?.Name ?? "Şirket bilgisi eksik";

            return invoiceDto;
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

        public async Task<(IEnumerable<InvoiceDTO> Invoices, int TotalRecords)> GetInvoicesAsync(string series, int pageNumber, int pageSize)
        {
            var query = _context.Invoices
                .Include(i => i.Company)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceDetails) // Details de dahil edildi
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
                    TotalPrice = detail.Quantity * detail.UnitPrice // Hesaplama buraya taşındı
                }).ToList()
            });

            return (invoiceDtos, totalRecords);
        }
    }
}
