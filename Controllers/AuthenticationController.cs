using BookStore.Dtos;
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

        [Route("signup")]
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

    }
}
