using System.ComponentModel.DataAnnotations;

public class Company
{
    public int ID { get; set; }

    [Required(ErrorMessage = "Şirket adı zorunludur.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Adres zorunludur.")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçersiz telefon numarası formatı.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
    public string Email { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
