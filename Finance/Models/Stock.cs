using System;

public class Stock
{
    public int ID { get; set; }
    public int CompanyID { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }

    public Company Company { get; set; }
    public ICollection<InvoiceDetails> InvoiceDetails { get; set; }
    public ICollection<StockTrans> StockTrans { get; set; }
    public ICollection<Balance> Balances { get; set; }
}

