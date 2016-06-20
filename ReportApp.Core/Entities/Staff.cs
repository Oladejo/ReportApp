using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ReportApp.Core.Entities
{
    public class Staff: IdentityUser
    {
        public virtual Profile Profile { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Staff> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
