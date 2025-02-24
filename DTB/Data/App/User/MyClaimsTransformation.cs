namespace DTB.Data.App.User
{
    using Microsoft.AspNetCore.Authentication;
    using System.Security.Claims;

    public class MyClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var claimType = "myCustomClaim";

            // 仅在 ClaimsPrincipal 中没有该声明时添加
            if (!principal.HasClaim(claim => claim.Type == claimType))
            {
                claimsIdentity.AddClaim(new Claim(claimType, "myClaimValue"));
            }

            principal.AddIdentity(claimsIdentity);
            return Task.FromResult(principal);
        }
    }
}
