using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityProvider.Enums;
using IdentityProvider.Helpers;
using IdentityProvider.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider
{
    public class IdentityWithAdditionalClaimsProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public IdentityWithAdditionalClaimsProfileService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> 
            roleManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = await _userManager.FindByIdAsync(sub);

            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();

            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            }

            claims.Add(new Claim(JwtClaimTypes.GivenName, user.Firstname));
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.Lastname));
            claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber));

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any(roleName => roleName == "wanvet.admin"))
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "wanvet.admin"));
            }

            if (roles.Any(roleName => roleName == "wanvet.staff"))
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "wanvet.staff"));
            }

            if (roles.Any(roleName => roleName == "wanvet.doctor"))
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "wanvet.doctor"));
            }

            claims.Add(new Claim(JwtClaimTypes.Role, "wanvet.user"));

            claims.Add(new Claim(StandardScopes.Email.Name, user.Email));

            var genderEnum = (Gender)Enum.ToObject(typeof(Gender), user.Gender);

            claims.Add(new Claim(JwtClaimTypes.Gender, genderEnum.GetDescription()));

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