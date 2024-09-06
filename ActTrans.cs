using System;

public class ActTrans
{
    public int ID { get; set; } 
    public int CustomerID { get; set; } 
    public int InvoiceID { get; set; } 
    public string TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Customer Customer { get; set; }
    public Invoice Invoice { get; set; }
}
