namespace BookStore.Services.Jwt
{
    public interface IJwtService
    {
        string GenerateToken(string username, long time);
        string ValidateToken(string token);
    }
}
