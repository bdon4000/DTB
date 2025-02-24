using Microsoft.AspNetCore.Identity;

namespace DTB.Data.App.User
{
    public class UserService
    {

        private readonly UserManager<AppUser> _userManager;
        

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }


        public  List<AppUser> GetList() => _userManager.Users.ToList();

        public static List<string> GetRoleList() => new List<string>  
        {
            "Operator", "Technician", "Admin", "SuperAdmin", 
        };

        public static Dictionary<string, string> GetRoleIconMap() => new()
        {
            ["Admin"] = "mdi-pencil,info",
            ["Operator"] = "mdi-account,pry",
            ["SuperAdmin"] = "mdi-account-edit,error",
            ["Technician"] = "mdi-cog,remind",
        };

        //public static List<string> GetPlanList() => new List<string>
        //{
        //    "Basic", "Company", "Enterprise", "Team",
        //};

        public static List<string> GetStatusList() => new List<string>
        {
            "InActive", "Active", "Working",
        };

        //public static List<string> GetLanguageList() => new List<string>
        //{
        //    "English", "Spanish", "French", "Russian", "German", "Arabic","Sanskrit",
        //};

        //public static List<PermissionDto> GetPermissionsList() => new List<PermissionDto>()
        //{
        //    new PermissionDto() { Module="Admin", Read = true },
        //    new PermissionDto() { Module="Staff", Write = true },
        //    new PermissionDto() { Module="Author", Read = true, Create = true },
        //    new PermissionDto() { Module="Contributor" },
        //    new PermissionDto() { Module="User", Delete = true },
        //};


    }

}
