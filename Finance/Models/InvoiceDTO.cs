namespace Finance.Models
{
    public class InvoiceDTO
    {
        public int ID { get; set; } // Fatura ID'si
        public int CustomerID { get; set; }  // Müşteri ID'si
        public string CustomerName { get; set; } // Müşteri Adı
        public int CompanyID { get; set; }  // Şirket ID'si
        public string CompanyName { get; set; } // Şirket Adı
        public DateTime InvoiceDate { get; set; }  // Fatura tarihi
        public string Series { get; set; }  // Fatura serisi
        public decimal TotalAmount { get; set; }  // Toplam tutar
        public List<InvoiceDetailsDTO> InvoiceDetails { get; set; }  // Fatura detayları
        public string Status { get; set; } = "Taslak";  // Varsayılan olarak "Taslak"
    }
}
