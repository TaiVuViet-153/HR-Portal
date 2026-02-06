using Auth.Application.DTOs;
using Auth.Application.ValueObjects;

namespace Auth.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResult> Login(LoginRequest loginRequest);
    Task<RegisterResult> Register(RegisterRequest registerRequest);
    Task<bool> RevokeRefreshToken(string refreshToken);
    Task<LoginResult> UpdateRefreshToken(string refreshToken);
}