using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace DTB.Components.Account.Pages
{
    public partial class Login
    {
        private bool _show;
        public string? UserName;
        public string Password;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public bool HideLogo { get; set; }

        [Parameter]
        public double Width { get; set; } = 410;

        [Parameter]
        public StringNumber? Elevation { get; set; }

        [Parameter]
        public string CreateAccountRoute { get; set; } = $"Account/Register";

        [Parameter]
        public string ForgotPasswordRoute { get; set; } = $"pages/authentication/forgot-password-v1";

        private async Task LoginClicked()
        {
            //error = null;
            var usr = await UserManager.FindByNameAsync(UserName);
  
            if (usr == null)
            {
                await PopupService.EnqueueSnackbarAsync("用户不存在", AlertTypes.Success);
                return;
            }


            if (await SignInManager.CanSignInAsync(usr))
            {
                var result = await SignInManager.CheckPasswordSignInAsync(usr, Password, true);
                if (result == Microsoft.AspNetCore.Identity.SignInResult.Success)
                {
                    Guid key = Guid.NewGuid();
                    // 获取用户的所有声明
                    var claims = await UserManager.GetClaimsAsync(usr);
                    // 检查用户是否已经有JobNumber的声明
                    Claim jobNumberClaim = claims.FirstOrDefault(c => c.Type == "JobNumber");
                    if (jobNumberClaim == null || jobNumberClaim.Value != usr.JobNumber)
                    {
                        // 如果没有JobNumber的声明，或者声明的值与usr.JobNumber不同，则添加或更新声明
                        if (jobNumberClaim != null)
                        {
                            // 如果声明存在，则先移除旧的声明
                            await UserManager.RemoveClaimAsync(usr, jobNumberClaim);
                        }
                        // 添加新的声明
                        await UserManager.AddClaimAsync(usr, new Claim("JobNumber", usr.JobNumber));
                    }
                    // 检查用户是否已经有Avatar的声明
                    Claim avatarClaim = claims.FirstOrDefault(c => c.Type == "Avatar");
                    if (avatarClaim == null || avatarClaim.Value != usr.Avatar)
                    {
                        // 如果没有Avatar的声明，或者声明的值与usr.Avatar不同，则添加或更新声明
                        if (avatarClaim != null)
                        {
                            // 如果声明存在，则先移除旧的声明
                            await UserManager.RemoveClaimAsync(usr, avatarClaim);
                        }
                        // 添加新的声明
                        await UserManager.AddClaimAsync(usr, new Claim("Avatar", usr.Avatar));
                    }
                    Claim roleClaim = claims.FirstOrDefault(c => c.Type == "role");
                    if (roleClaim == null || roleClaim.Value != usr.Avatar)
                    {
                        // 如果没有Avatar的声明，或者声明的值与usr.Avatar不同，则添加或更新声明
                        if (roleClaim != null)
                        {
                            // 如果声明存在，则先移除旧的声明
                            await UserManager.RemoveClaimAsync(usr, roleClaim);
                        }
                        // 添加新的声明
                        await UserManager.AddClaimAsync(usr, new Claim("role", usr.Role));
                    }
                    BlazorCookieLoginMiddleware.Logins[key] = new LoginInfo { UserName = UserName, Password = Password };
                    await PopupService.EnqueueSnackbarAsync("登录成功", AlertTypes.Success);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    NavigationManager.NavigateTo($"/login?key={key}", true);
                }
                else
                {
                    await PopupService.EnqueueSnackbarAsync("密码错误", AlertTypes.Success);
                }
            }
            else
            {
                //error = "Your account is blocked";
            }
        }
    }
}
