using System.Reflection.Metadata.Ecma335;

namespace BookStore.Dtos.UserDto
{
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Key { get; set; }

        public int Role {  get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
    }
}
