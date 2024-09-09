using System;

public class Invoice
{
    public int ID { get; set; }
    public int CompanyID { get; set; }
    public int CustomerID { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Series { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }

    public Company Company { get; set; }
    public Customer Customer { get; set; }
    public ICollection<InvoiceDetails> InvoiceDetails { get; set; }
    public ICollection<ActTrans> ActTrans { get; set; }
}
