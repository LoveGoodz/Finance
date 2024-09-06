using System;

public class StockTrans
{
    public int ID { get; set; }
    public int StockID { get; set; }
    public int InvoiceDetailsID { get; set; } 
    public string TransactionType { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Stock Stock { get; set; }
    public InvoiceDetails InvoiceDetails { get; set; }
}
