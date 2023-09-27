namespace E_Commerce_API.Models
{
    public class UserRegisterModel
    {   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ShippingAddress { get; set; }
    }
}
