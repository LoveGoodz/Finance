using System;

public class InvoiceDetails
{
    public int ID { get; set; }
    public int InvoiceID { get; set; }
    public int StockID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }

    public Invoice Invoice { get; set; }
    public Stock Stock { get; set; }
}
