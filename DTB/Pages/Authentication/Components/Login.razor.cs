using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using DTB.Data.App.User;

namespace DTB.Pages.Authentication.Components
{

    public partial class Login
    {
        private bool _show;

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

        [Parameter]
        public EventCallback<MouseEventArgs> OnLogin { get; set; }
    }

}
