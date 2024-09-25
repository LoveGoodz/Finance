namespace Finance.Models
{
    public class StockDTO
    {
        public int CompanyID { get; set; }  

        public string Name { get; set; }  
        public int Quantity { get; set; }  
        public double UnitPrice { get; set; }  

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
