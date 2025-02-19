namespace BookStore.Services.Jwt
{
    public interface IJwtService
    {
        // time = minutes
        string GenerateToken(string username, long time);
        string ValidateToken(string token);
    }
}
