namespace BookStore.Dtos.UserDto
{
    public class UserLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}
