namespace Finance.Models
{
    public class InvoiceDTO
    {
        public int CustomerID { get; set; }  // Müşteri ID'si
        public int CompanyID { get; set; }  // Şirket ID'si
        public DateTime InvoiceDate { get; set; }  // Fatura tarihi
        public string Series { get; set; }  // Fatura serisi
        public decimal TotalAmount { get; set; }  // Toplam tutar
        public List<InvoiceDetailsDTO> InvoiceDetails { get; set; }  // Fatura detayları

        public string Status { get; set; } = "Taslak";  // Varsayılan olarak "Taslak"
    }
}
