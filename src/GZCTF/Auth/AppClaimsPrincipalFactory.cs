using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using GZCTF.Models.Data;   // nơi khai báo UserInfo (theo stack trace của bạn)
using GZCTF.Utils;        // enum Role bạn vừa thêm Teacher = 4

public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<UserInfo>
{
    public AppClaimsPrincipalFactory(
        UserManager<UserInfo> userManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UserInfo user)
    {
        var id = await base.GenerateClaimsAsync(user);
        id.AddClaim(new Claim(id.RoleClaimType, user.Role.ToString())); // "Admin"/"Teacher"/"User"
        id.AddClaim(new Claim("role", user.Role.ToString()));           // FE dùng claim này
        id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        return id;
    }
}
