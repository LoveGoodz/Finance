using Finance.Data;
using Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Services
{
    public class InvoiceDetailsService : IInvoiceDetailsService
    {
        private readonly FinanceContext _context;

        public InvoiceDetailsService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDetails> GetInvoiceDetailsByIdAsync(int id)
        {
            return await _context.InvoiceDetails
                .Include(d => d.Invoice)
                .Include(d => d.Stock)
                .FirstOrDefaultAsync(d => d.ID == id);
        }

        public async Task<InvoiceDetails> CreateInvoiceDetailsAsync(InvoiceDetailsDTO detailsDto)
        {
            var invoiceDetails = new InvoiceDetails
            {
                InvoiceID = detailsDto.InvoiceID,
                StockID = detailsDto.StockID,
                Quantity = detailsDto.Quantity,
                UnitPrice = detailsDto.UnitPrice,
                TotalPrice = detailsDto.Quantity * detailsDto.UnitPrice,
                CreatedAt = DateTime.UtcNow
            };

            _context.InvoiceDetails.Add(invoiceDetails);
            await _context.SaveChangesAsync();
            return invoiceDetails;
        }

        public async Task<bool> UpdateInvoiceDetailsAsync(int id, InvoiceDetailsDTO detailsDto)
        {
            var invoiceDetails = await _context.InvoiceDetails.FindAsync(id);
            if (invoiceDetails == null)
            {
                return false;
            }

            invoiceDetails.StockID = detailsDto.StockID;
            invoiceDetails.Quantity = detailsDto.Quantity;
            invoiceDetails.UnitPrice = detailsDto.UnitPrice;
            invoiceDetails.TotalPrice = detailsDto.Quantity * detailsDto.UnitPrice;
            invoiceDetails.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceDetailsAsync(int id)
        {
            var invoiceDetails = await _context.InvoiceDetails.FindAsync(id);
            if (invoiceDetails == null)
            {
                return false;
            }

            _context.InvoiceDetails.Remove(invoiceDetails);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
