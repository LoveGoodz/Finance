using System.ComponentModel.DataAnnotations;

namespace Finance.Models
{
    public class CustomerDTO
    {
        public int CompanyID { get; set; }

        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
