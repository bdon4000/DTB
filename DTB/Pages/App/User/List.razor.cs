using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Drawing;



namespace DTB.Pages.App.User
{
    public partial class List: ProComponentBase
    {
        public bool? _visible;
        public UserPage _userPage;
        private List<int> _pageSizes = new() { 10, 25, 50, 100 };
        private List<DataTableHeader<AppUser>> _headers = new();
        private readonly Dictionary<string, string> _roleIconMap = UserService.GetRoleIconMap();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _headers = new List<DataTableHeader<AppUser>>
            {
                new() { Text = I18n.T("UserName"), Value = nameof(AppUser.UserName) },
                new() { Text = I18n.T("Role"), Value = nameof(AppUser.Role) },
                new() { Text = I18n.T("JobNumber"), Value = nameof(AppUser.JobNumber) },
                new() { Text = I18n.T("Status"), Value = nameof(AppUser.Status) },
                new() { Text = I18n.T("Actions"), Value = "Action", Sortable = false }
            };

            _userPage = new(GetUserList());
            _userPage = new( GetUserList());
        }


        private void NavigateToDetails(string id)
        {
            Nav.NavigateTo($"/app/user/view/{id}");
        }

        private void NavigateToEdit(string id)
        {
            Nav.NavigateTo($"/app/user/edit/{id}");
        }
        private List<AppUser> GetUserList()
        {
            return UserManager.Users.ToList();
        }
        private async Task<List<AppUser>> GetUserListAsync()
        {
            // 确保 UserManager.Users 是 IQueryable<TUser> 类型
            var users = await UserManager.Users.ToListAsync();
            return users;
        }

        private string getColor(string status)
        {
            switch (status)
            {
                case "InActive":
                    return "error";
                case "Active":
                    return "info";
                case "Working":
                    return "sample-green";
                default:
                    return "pry";
            }
        }
        //重写
        //private void AddUserData(UserDto userData)
        //{
        //    _userPage.UserDatas.Insert(0, userData);
        //}
    }
}
