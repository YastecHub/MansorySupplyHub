using Microsoft.AspNet.Identity.EntityFramework;

namespace MansorySupplyHub.Entities
{
    public class ApplicationUsers : IdentityUser
    {
        public string FullName { get; set; }
    }
}

