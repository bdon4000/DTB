using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace DTB.Data.App.User
{
    public class LoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class BlazorCookieLoginMiddleware
    {
        //public static IDictionary<Guid, LoginInfo> Logins { get; private set; }
        //    = new ConcurrentDictionary<Guid, LoginInfo>();
        public static ConcurrentDictionary<Guid, LoginInfo> Logins { get; private set; }
    = new ConcurrentDictionary<Guid, LoginInfo>();

        private readonly RequestDelegate _next;

        public BlazorCookieLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, SignInManager<AppUser> signInMgr)
        {
            if (context.Request.Path == "/login" && context.Request.Query.ContainsKey("key"))
            {
                var key = Guid.Parse(context.Request.Query["key"]);
                //var info = Logins[key];
                LoginInfo info;
                if (Logins.TryGetValue(key, out info))
                {
                    var result = await signInMgr.PasswordSignInAsync(info.UserName, info.Password, false, lockoutOnFailure: true);
                    info.Password = null;
                    //if (result.Succeeded)
                    //{
                    //    var user = await signInMgr.UserManager.FindByNameAsync(info.UserName);
                    //    if (user != null)
                    //    {
                    //        // 创建 ClaimsPrincipal
                    //        var claimsPrincipal = await signInMgr.CreateUserPrincipalAsync(user);

                    //        // 添加自定义声明
                    //        var identity = (ClaimsIdentity)claimsPrincipal.Identity;
                    //        identity.AddClaim(new Claim("JobNumber", user.JobNumber));
                    //        identity.AddClaim(new Claim("Avatar", user.Avatar));

                    //        // 设置当前用户的身份
                    //        context.User = claimsPrincipal;

                    //        Logins.Remove(key);
                    //        context.Response.Redirect("/");
                    //        return;
                    //    }
                    //}
                    if (result.Succeeded)
                    {
                        var user = await signInMgr.UserManager.FindByNameAsync(info.UserName);
                        if (user != null)
                        {
                            var claimsPrincipal = await signInMgr.CreateUserPrincipalAsync(user);
                            var identity = (ClaimsIdentity)claimsPrincipal.Identity;
                            identity.AddClaim(new Claim("JobNumber", user.JobNumber));
                            identity.AddClaim(new Claim("Avatar", user.Avatar));
                            context.User = claimsPrincipal;

                            // Use TryRemove instead of Remove
                            LoginInfo removedInfo;
                            Logins.TryRemove(key, out removedInfo);

                            context.Response.Redirect("/");
                            return;
                        }
                    }
                }
                else
                {
                    //TODO: Proper error handling
                    context.Response.Redirect("/loginfailed");
                    return;
                }
            }
            else if (context.Request.Path.StartsWithSegments("/logout"))
            {
                await signInMgr.SignOutAsync();
                context.Response.Redirect("/");
                return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
