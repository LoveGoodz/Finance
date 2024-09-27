namespace Finance.Models
{
    public class InvoiceDTO
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; } 
        public int CompanyID { get; set; }
        public string? CompanyName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Series { get; set; }
        public decimal TotalAmount { get; set; }
        public List<InvoiceDetailsDTO> InvoiceDetails { get; set; }
        public string Status { get; set; } = "Taslak";
    }
}
