using System;

public class Balance
{
    public int ID { get; set; } 
    public int CompanyID { get; set; }
    public int StockID { get; set; } 
    public int CustomerID { get; set; }
    public decimal TotalStock { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; }

    public Company Company { get; set; }
    public Stock Stock { get; set; }
    public Customer Customer { get; set; }
}
