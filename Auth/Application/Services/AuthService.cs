using Auth.Application.DTOs;
using Auth.Application.Services.Interfaces;
using Auth.Application.ValueObjects;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Auth.Domain.Services;

namespace Auth.Application.Services;

public class AuthService(
    IAuthRepository authRepository,
    ITokenService tokenService
) : IAuthService
{

    public async Task<LoginResult> Login(LoginRequest userLogin)
    {
        var errMsg = "Login failed";
        var LockedMsg = "Your account is locked. Please contact the administrator to reset password.";

        var user = await authRepository.GetByUserNameAsync(userLogin.Username);

        if (user == null)
            return new LoginResult(null, errMsg);

        //verify password & update fail attempts if wrong password
        if (!user.VerifyPassword(userLogin.Password))
        {
            var attempts = await authRepository.UpdateFailAttempts(user);
            return new LoginResult(null, (attempts == 3) ? LockedMsg : errMsg);
        }

        if (
            user.Status == User.UserStatus.Locked &&
            user.FailedLoginCount == 3 &&
           !(user.requiredChangePW ?? false) // requiredChangePW is true when admin click reset account
        )
            return new LoginResult(null, LockedMsg);

        //update some variable if login successfully
        await authRepository.HandleSuccessfulLogin(user);

        // create accessToken
        var accessToken = await tokenService.GenerateAccessToken(user);

        // create refresh token
        var refreshToken = tokenService.GenerateRefreshToken();

        await authRepository.AddRefreshTokenAsync(user.UserID, refreshToken);

        // get role by userid
        var roles = await authRepository.GetRoles(user.UserID);

        // Login result return model
        var userResult = new UserResult
        (
            user.UserID,
            user.UserName,
            string.Join(",", roles),
            accessToken,
            refreshToken,
            user.requiredChangePW
        );

        return new LoginResult(userResult, "");
    }

    public async Task<RegisterResult> Register(RegisterRequest registerRequest)
    {
        var isExistUsername = "Username is existed";

        var user = await authRepository.GetByUserNameAsync(registerRequest.Username);

        if (user != null) return new RegisterResult(false, isExistUsername);

        var newUser = new User(registerRequest.Username, registerRequest.Email);
        newUser.SetPassword(registerRequest.Password);
        newUser.requiredChangePW = false;
        newUser.Status = User.UserStatus.Newly_Created;

        var result = await authRepository.CreateUser(newUser);

        var errMsg = Convert.ToBoolean(result) ? "" : "Register failed";

        return new RegisterResult(Convert.ToBoolean(result), errMsg);
    }

    public async Task<bool> RevokeRefreshToken(string refreshToken)
    {
        var token = await authRepository.GetRefreshToken(refreshToken);

        if (token == null)
            throw new Exception("Refresh token not found");

        token.RevokeToken();

        return Convert.ToBoolean(await authRepository.SaveChangesAsync());
    }

    public async Task<LoginResult> UpdateRefreshToken(string refreshToken)
    {
        var token = await authRepository.GetRefreshToken(refreshToken);
        if (token == null)
            throw new Exception("Invalid refresh token");

        // revoke old refresh token
        var isRevoked = await RevokeRefreshToken(refreshToken);
        if (!isRevoked)
            throw new Exception("Failed to revoke old refresh token");

        // generate new refresh token
        var newRefreshToken = tokenService.GenerateRefreshToken();
        var user = await authRepository.GetByUserIdAsync(token.UserID);

        if (user == null)
            throw new Exception("No user to revoke old refresh token");

        var newAccessToken = await tokenService.GenerateAccessToken(user);


        // save new refresh token
        await authRepository.AddRefreshTokenAsync(token.UserID, newRefreshToken);

        // get role by userid
        var roles = await authRepository.GetRoles(token.UserID);



        return new LoginResult(
            new UserResult(
                user.UserID,
                user.UserName,
                string.Join(",", roles),
                newAccessToken,
                newRefreshToken,
                user.requiredChangePW
            ),
            ""
        );
    }
}