using Microsoft.AspNetCore.Identity;

namespace IdentityPro.Models
{
    public class appUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Apartment { get; set; }
        public int ZipCode { get; set; }
    }


}
