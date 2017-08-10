using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoCore.Models;

namespace TodoCore
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityWithAdditionalClaimsProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = await _userManager.FindByIdAsync(sub);

            //if (!await _userManager.IsInRoleAsync(user, "admin"))
            //{
            //    await _userManager.AddToRoleAsync(user, "admin");
            //}

            var userRoles = await _userManager.GetRolesAsync(user);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();

            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();


            claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
            //claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
            //new Claim(JwtClaimTypes.Role, "dataEventRecords.admin"),
            //new Claim(JwtClaimTypes.Role, "dataEventRecords.user"),
            //new Claim(JwtClaimTypes.Role, "dataEventRecords"),
            //new Claim(JwtClaimTypes.Role, "securedFiles.user"),
            //new Claim(JwtClaimTypes.Role, "securedFiles.admin"),
            //new Claim(JwtClaimTypes.Role, "securedFiles")

            Console.WriteLine("**********************************************************************************");
            Console.WriteLine("**********************************************************************************");
            Console.WriteLine("**********************************************************************************");
            Console.WriteLine("**********************************************************************************");
            Console.WriteLine("roles count:  " + user.Roles.Count());
            Console.WriteLine("roles count:  " + userRoles.Count());
            Console.WriteLine("usre name:  " + user.UserName);
            foreach (var role in userRoles)
            {
                Console.WriteLine("roles role:  " + role.ToString());
                claims.Add(new Claim(JwtClaimTypes.Role, role.ToString()));
            }


            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));


            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
