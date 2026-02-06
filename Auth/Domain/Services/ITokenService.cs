using Auth.Domain.Entities;

namespace Auth.Domain.Services;

public interface ITokenService
{
    Task<string> GenerateAccessToken(User userLogin);
    string GenerateRefreshToken(int size = 64);
}