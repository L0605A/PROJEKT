using CodeFirst.Models;

namespace CodeFirst.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(UserDto request);
    Task<AuthResult> LoginAsync(UserDto request);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
}
