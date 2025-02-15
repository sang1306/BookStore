using System.IdentityModel.Tokens.Jwt;
using BookStore.Dtos.UserDto;
using BookStore.Models;
using BookStore.Services;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("Auth")]
    public class AuthenticationController : Controller
    {
        private readonly UserService _userService;
        public AuthenticationController(UserService userService)
        {
            _userService = userService;
        }


        //[TypeFilter(typeof(AuthenticationFilter))]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginRequest login)
        {
            UserLoginResponse response = await _userService.UserLogin(login, HttpContext);
            return Json(response);
        }

        [Route("SignUp")]
        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpRequest signup)
        {
            UserSignUpResponse response = await _userService.UserSignUp(signup);
            return Json(response);
        }

        [Route("SignOut")]
        [HttpGet]
        public IActionResult SignOut()
        {
            UserSessionManager.RemoveUserInfo(HttpContext);
            return RedirectToAction(nameof(Index));
        }

        [Route("ValidateToken")]
        [HttpPost]
        public IActionResult ValidateToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValid = _userService.ValidateToken(token, HttpContext); 

            return Json(new { isAuthenticated = isValid });
        }
    }
}
