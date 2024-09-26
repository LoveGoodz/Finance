using System;
using System.Text.Json.Serialization;

public class Invoice
{
    public int ID { get; set; }
    public int CompanyID { get; set; }
    public int CustomerID { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Series { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Company Company { get; set; }

    [JsonIgnore]
    public Customer Customer { get; set; }

    [JsonIgnore]
    public ICollection<InvoiceDetails> InvoiceDetails { get; set; }

    [JsonIgnore]
    public ICollection<ActTrans> ActTrans { get; set; }
}