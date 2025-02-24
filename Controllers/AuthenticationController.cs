using System.IdentityModel.Tokens.Jwt;
using BookStore.Dtos.UserDto;
using BookStore.Models;
using BookStore.Services;
using BookStore.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("Auth")]
    public class AuthenticationController : Controller
    {
        private readonly AuthService _userService;
        public AuthenticationController(AuthService userService)
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
        [Route("ForgotPass")]
        [HttpGet]
        public IActionResult ForgotPass()
        {
            return View();
        }
        [Route("ForgotPass")]
        [HttpPost]
        public IActionResult ForgotPass(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                TempData["mess"] = "Please enter an email address.";
                return View();
            }
            UserLoginResponse mess = _userService.ForgotPass(key);
            if (mess.Success)
            {
                TempData["success"] = mess.Message;
            }
            TempData["mess"] = mess.Message;
            return View();
        }
        [Route("ResetPassword")]
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            // check token, time
            var check = _userService.CheckToken(token);
            if (string.IsNullOrEmpty(check))
            {
                // invalid return error page + message 
                // action: error, controller: home
                return RedirectToAction("error", "home");
            }
            ViewBag.token = token;
            // valid return page
            return View();
        }
        [Route("ResetPassword")]
        [HttpPost]
        public IActionResult ResetPassword(string password, string repassword, string token)
        {
            var check = _userService.ResetPassWord(password, repassword, token);
            if (!check)
            {
                ViewBag.mess = "Seem like error token, or not matching password";
            }
            TempData["mess"] = "reset your password success";
            return RedirectToAction("Index");
        }

    }
}
