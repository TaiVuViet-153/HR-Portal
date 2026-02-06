using Auth.Domain.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly byte[] _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IAuthRepository _authRepository;

    public TokenService(IConfiguration config, IAuthRepository authRepository)
    {
        _config = config;
        _key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new ArgumentException("Secret Key not set"));
        _issuer = _config["Jwt:Issuer"];
        _audience = _config["Jwt:Audience"];
        _authRepository = authRepository;
    }

    public async Task<string> GenerateAccessToken(User userLogin)
    {
        // Claims
        var claims = new List<Claim>
        {

            new Claim(ClaimTypes.NameIdentifier, userLogin.UserID.ToString()),
            new Claim(ClaimTypes.Email, userLogin.Email),
            new Claim(JwtRegisteredClaimNames.Sub, userLogin.UserName),   // Subject of token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID of token
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()) // Created time of token
        };

        // Get UserRoles
        var userRoles = await _authRepository.GetRoles(userLogin.UserID);

        // Get Permissions
        var rolePermissions = await _authRepository.GetPermissions(userLogin.UserID);

        // Add Claims for role
        foreach (var role in userRoles) claims.Add(new Claim(ClaimTypes.Role, role));

        // Add Claims for permission
        foreach (var permission in rolePermissions) claims.Add(new Claim("Permission", permission));

        // create a secret credentails
        var credentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature);

        // Token string preparing
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpiryMinutes"])), // Expired time is 60 minutes
            SigningCredentials = credentials,
            Issuer = _issuer,                 // Add Issuer
            Audience = _audience,              // Add Audience
        };

        // create access token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // return access token
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(int size = 64)
    {
        var randomNumber = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(randomNumber);
    }
}