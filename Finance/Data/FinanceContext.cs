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
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Company tablosu ilişkileri
            modelBuilder.Entity<Company>().ToTable("Company");

            // Customer tablosu ilişkileri
            modelBuilder.Entity<Customer>().ToTable("Customer")
                .HasOne(c => c.Company)
                .WithMany(c => c.Customers)
                .HasForeignKey(c => c.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice tablosu ilişkileri ve datetime2 kullanımı
            modelBuilder.Entity<Invoice>().ToTable("Invoice")
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>().Property(i => i.InvoiceDate).HasColumnType("datetime2");
            modelBuilder.Entity<Invoice>().Property(i => i.CreatedAt).HasColumnType("datetime2");
            modelBuilder.Entity<Invoice>().Property(i => i.UpdatedAt).HasColumnType("datetime2");

            // Stock tablosu
            modelBuilder.Entity<Stock>().ToTable("Stock");

            // Balance tablosu
            modelBuilder.Entity<Balance>().ToTable("Balance")
                .Property(b => b.CreatedAt).HasColumnType("datetime2");
            modelBuilder.Entity<Balance>().Property(b => b.UpdatedAt).HasColumnType("datetime2");

            // StockTrans tablosu
            modelBuilder.Entity<StockTrans>().ToTable("StockTran")
                .Property(st => st.CreatedAt).HasColumnType("datetime2");

            // ActTrans tablosu
            modelBuilder.Entity<ActTrans>().ToTable("ActTran")
                .Property(at => at.CreatedAt).HasColumnType("datetime2");

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
