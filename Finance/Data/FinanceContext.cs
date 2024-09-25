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

        // DbSet tanımları
        public DbSet<Company> Companies { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }
        public DbSet<StockTrans> StockTrans { get; set; }
        public DbSet<ActTrans> ActTrans { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Customer>().ToTable("Customer")
                .HasOne(c => c.Company)
                .WithMany(c => c.Customers)
                .HasForeignKey(c => c.CompanyID); 

            modelBuilder.Entity<Company>()
                .ToTable("Company")
                .HasMany(c => c.Customers)
                .WithOne(c => c.Company)
                .HasForeignKey(c => c.CompanyID);

            modelBuilder.Entity<Stock>().ToTable("Stock");
            modelBuilder.Entity<Invoice>().ToTable("Invoice");
            modelBuilder.Entity<Balance>().ToTable("Balance");
            modelBuilder.Entity<StockTrans>().ToTable("StockTran");
            modelBuilder.Entity<ActTrans>().ToTable("ActTran");

            // Varsayılan kullanıcı seed işlemi
            modelBuilder.Entity<User>().HasData(new User
            {
                ID = 1,
                Username = "admin",
                Password = "123456", 
                Role = RoleType.Admin
            });
        }
    }
}
