using System.Reflection.Metadata.Ecma335;

namespace BookStore.Dtos
{
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Key { get; set; } 
    }
}
