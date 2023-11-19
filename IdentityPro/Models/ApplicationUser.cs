using Microsoft.AspNetCore.Identity;

namespace IdentityPro.Models
{
    public class ApplicationUser : IdentityUser
    {
        // You can add additional properties here to extend the user profile, for example:
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
