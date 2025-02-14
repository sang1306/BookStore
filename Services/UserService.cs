using System.ComponentModel.DataAnnotations;
using BookStore.Dtos;
using BookStore.Enums;
using BookStore.Models;
using BookStore.Utils;
using chat_application_demo.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class UserService
    {

        private readonly Prn222BookshopContext _context;
        public UserService(Prn222BookshopContext context)
        {
            _context = context;
        }

        public async Task<UserLoginResponse> UserLogin(UserLoginRequest request, HttpContext httpContext)
        {
            // hash password to check 
            string passRequest = StringUtils.ComputeSha256Hash(request.Password);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (user != null)
            {
                Console.WriteLine("passrequest: " + passRequest);
                Console.WriteLine(": " + passRequest);
                if (passRequest == user.Password)
                {
                    // add session 
                    UserSessionManager.SetUserInfo(httpContext, user);
                    // generate key

                    // return success
                    return new UserLoginResponse()
                    {
                        Success = true,
                        Message = "Login successful!",
                        Key = "asdfasfwer2323"
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
                Message = "User registered successfully"
            };
        }

        public void UserSignOut(HttpContext httpContext)
        {
        }
    }
}
