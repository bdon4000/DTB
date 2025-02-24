using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Microsoft.AspNetCore.Identity;
using DTB.Pages.App.User;
using Microsoft.AspNetCore.SignalR.Client;
using OneOf.Types;
using Microsoft.AspNetCore.Http.HttpResults;
using DTB.Data.App.User;

namespace DTB.Controllers
{
    public class UserLoginInfo
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        
    }

    public class ResponseMessage
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public string Role { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserLoginInfo userLoginInfo)
        {
            var responseMessage = new ResponseMessage();
            var usr = await _userManager.FindByNameAsync(userLoginInfo.UserName);
            if (usr == null)
            {
                responseMessage.Code = 404;
                responseMessage.Message = "User Not Found";// error = "User not found";
            }

            else if (await _signInManager.CanSignInAsync(usr))
            {
                var result = await _signInManager.CheckPasswordSignInAsync(usr, userLoginInfo.Password, true);
                if (result.Succeeded)
                {
                    responseMessage.Code = 200;
                    responseMessage.Role = usr.Role;
                    responseMessage.Message = "Login success";
                }
                else
                {
                    responseMessage.Code = 406;
                    responseMessage.Message = "密码错误";
                }

            }
            return Ok(responseMessage);

        }
    }
}
