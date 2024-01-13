
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tangehrine.DataLayer.DbContext;

namespace Tangehrine.Common.Identity
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, Role>
    {
        public ClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().FirstOrDefault()?.Value ?? "";
            
            var claims = new List<Claim>()
            {
                new Claim("UserRole", role),
                new Claim("UserId", user.Id.ToString() ?? ""),
                new Claim("UserName", user.UserName ??"" )
              
            };
            identity.AddClaims(claims);
            return identity;
        }
    }
}
