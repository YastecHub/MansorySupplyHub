using Microsoft.AspNetCore.Identity;

namespace MansorySupplyHub.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}

