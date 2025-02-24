using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using Azure.Core;
using BookStore.Dtos.UserDto;
using BookStore.Enums;
using BookStore.Models;
using BookStore.Services.Jwt;
using BookStore.Services.Mail;
using BookStore.Utils;
using chat_application_demo.Utils;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace BookStore.Services
{
    public class AuthService
    {

        private readonly Prn222BookshopContext _context;
        private readonly IJwtService _jwtService;
        private readonly MailService _mailService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public AuthService(Prn222BookshopContext context, IJwtService jwtService, MailService mailService, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _context = context;
            _jwtService = jwtService;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public async Task<UserLoginResponse> UserLogin(UserLoginRequest request, HttpContext httpContext)
        {
            // hash password to check 
            string passRequest = StringUtils.ComputeSha256Hash(request.Password);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (user != null)
            {
                if (passRequest == user.Password)
                {
                    // add session 
                    UserSessionManager.SetUserInfo(httpContext, user);

                    // generate key if have remember me
                    string token = "";
                    if (request.Remember)
                    {
                        long oneweek = 60 * 24 * 7;
                        token = _jwtService.GenerateToken(user.Username, oneweek);
                    }
                    // return success
                    return new UserLoginResponse()
                    {
                        Success = true,
                        Message = "Login successful!",
                        Key = token
                    };
                }
                else
                {
                    return new UserLoginResponse()
                    {
                        Success = false,
                        Message = "Wrong password!",
                    };
                }
            }
            return new UserLoginResponse()
            {
                Success = false,
                Message = "Invalid username!",
            };

        }
        public async Task<UserSignUpResponse> UserSignUp(UserSignUpRequest request)
        {

            var validationContext = new ValidationContext(request);
            var validateResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(request, validationContext, validateResults, true);
            if (!isValid)
            {
                return new UserSignUpResponse()
                {
                    Success = false,
                    Message = string.Join(", ", validateResults.Select(x => x.ErrorMessage))
                };

            }

            // Check for existing username or email in a single query
            var existingUser = await _context.Users
                .Where(x => x.Username == request.Username || x.Email.ToLower() == request.Email.ToLower())
                .Select(x => new { x.Username, x.Email })
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                string message = existingUser.Username == request.Username
                    ? "Username already exists!" : "Email already exists!";

                return new UserSignUpResponse
                {
                    Success = false,
                    Message = message
                };
            }

            // Create new user
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = StringUtils.ComputeSha256Hash(request.Password),
                CreateAt = DateTime.UtcNow,
                Status = 1,
                Role = (int)Roles.User
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new UserSignUpResponse()
            {
                Success = true,
                Message = "User registered  successfully"
            };
        }


        public async Task<bool> ValidateToken(string token, HttpContext httpContext)
        {
            string usename = _jwtService.ValidateToken(token);
            if (usename != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == usename);
                if (user != null)
                {
                    UserSessionManager.SetUserInfo(httpContext, user);
                    return true;
                }
            }
            return false;
        }

        public UserLoginResponse ForgotPass(string key)
        {
            // Check for existing username or email in a single query
            var existingUser = _context.Users
            .Where(x => x.Username == key || x.Email.ToLower() == key.ToLower())
            .Select(x => new { x.Username, x.Email })
                .FirstOrDefault();

            if (existingUser != null)
            {
                // generate token
                string token = _jwtService.GenerateToken(existingUser.Username, 5);

                string linkToken = GeneratePasswordResetUrl(token);



                // send mail
                string message = "If an account exists with this " + existingUser.Username == key ? "username" : "email" + ", you will receive password reset instructions.";

                String content = "<div style='text-align: center; font-family: Consolas; background-color:#1b263b; color:#e0e1dd; padding: 20px;border-radius: 10px'>\n" +
                        "    <h1 style='margin: 10px'>Hey is you forget Pass!</h1>\n" +
                        "    <nav style='margin: 20px;font-size: 12px; color:orangered'>*if you not require to change password, you can ignore this\n" +
                        "        email!\n" +
                        "    </nav>\n" +
                        "\n" +
                        "    <h4>Click link below to reset you password</h4>\n" +
                        "    <a style='padding:10px 20px;background-color:#e0e1dd; color: #0d1b2a; text-decoration: none; border-radius: 5px'\n" +
                        "       href='" + linkToken + "'>here</a>\n" +
                        "</div>\n";
                var header = "Reset your password | BookStore";

                _mailService.SendMail(content, existingUser.Email, header);

                return new UserLoginResponse
                {
                    Success = true,
                    Message = message
                };
            }
            else
            {
                return new UserLoginResponse()
                {
                    Message = "Not found any email or username",
                    Success = false
                };
            }
        }
        private string GeneratePasswordResetUrl(string token)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var resetLink = _linkGenerator.GetPathByAction(
                action: "ResetPassword",
                controller: "Authentication",
                values: new { token });

            return $"{baseUrl}{resetLink}";
        }

        public string CheckToken(string token)
        {
            return _jwtService.ValidateToken(token);
        }

        public bool ResetPassWord(string password, string repassword, string token)
        {
            // check token , time again
            var username = _jwtService.ValidateToken(token);
            if (string.IsNullOrEmpty(username)) { return false; }

            // find user
            var existUser = _context.Users.FirstOrDefault(x => x.Username.Equals(username));
            if (existUser == null || password != repassword) return false;

            // change password, save change
            existUser.Password = StringUtils.ComputeSha256Hash(password);
            _context.SaveChanges();

            return true;
        }
    }
}
