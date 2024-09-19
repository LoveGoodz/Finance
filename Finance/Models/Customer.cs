using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Customer
{
    public int ID { get; set; }

    [Required]
    public int CompanyID { get; set; } 

    [Required]
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }

    [JsonIgnore]
    public Company Company { get; set; } 
}
