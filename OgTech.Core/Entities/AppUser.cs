using Microsoft.AspNetCore.Identity;

namespace OgTech.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }

}
