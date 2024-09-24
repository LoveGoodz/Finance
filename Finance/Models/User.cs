namespace Finance.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }  
        public RoleType Role { get; set; }  
    }

}
