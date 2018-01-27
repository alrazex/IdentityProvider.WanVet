using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityProvider.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public bool IsActive { get; set; }

        public int Gender { get; set; }

    }
}