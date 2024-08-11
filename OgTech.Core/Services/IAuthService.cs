using Microsoft.AspNetCore.Identity;
using OgTech.Core.Entities;

namespace OgTech.Core.Services
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager);
    }
}
