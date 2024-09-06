using Microsoft.EntityFrameworkCore;
using Finance.Models;

namespace Finance.Data
{
    public class FinanceContext : DbContext
    {
        public FinanceContext(DbContextOptions<FinanceContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public DbSet<StockTrans> StockTrans { get; set; }
        public DbSet<ActTrans> ActTrans { get; set; }
        public DbSet<Balance> Balances { get; set; }
    }
}
