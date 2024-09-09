using System;

public class Company
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }

    public ICollection<Stock> Stocks { get; set; }
    public ICollection<Customer> Customers { get; set; }
    public ICollection<Invoice> Invoices { get; set; }
    public ICollection<Balance> Balances { get; set; }
}
