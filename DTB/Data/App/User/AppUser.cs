using Microsoft.AspNetCore.Identity;

namespace DTB.Data.App.User
{
    public class AppUser:IdentityUser
    {
        public string? FullName { get; set;}
        public string? JobNumber { get; set;}
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }


}
