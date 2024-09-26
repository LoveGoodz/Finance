using System;
using System.Text.Json.Serialization;

public class InvoiceDetails
{
    public int ID { get; set; }
    public int InvoiceID { get; set; }
    public int StockID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public Invoice Invoice { get; set; }

    public Stock Stock { get; set; }
}
