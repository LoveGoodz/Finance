namespace Finance.Models
{
    public class InvoiceDetailsDTO
    {
        public int InvoiceID { get; set; }
        public int StockID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; } // `set` eklenerek `null` hatası önlenir
    }
}
