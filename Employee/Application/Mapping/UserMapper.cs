using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Employee.Domain.Entities;
using Employee.Domain.ValueObjects;
using Request.Application.DTOs;

namespace Employee.Application.Mapping;

public static class UserMapper
{
    public static CreateUserResponse ToViewModel(this User user, string password)
    {
        return new CreateUserResponse
        {
            UserID = user.UserID,
            UserName = user.UserName,
            Email = user.Email,
            Password = password
        };
    }

    public static UpdateUserResponse ToViewModel(this User user)
    {
        return new UpdateUserResponse
        {
            UserID = user.UserID,
            UserName = user.UserName,
            Email = user.Email,
            Detail = user.Detail,
            Status = (byte)user.Status
        };
    }

    public static (bool, string) ToEntity(this User user, UpdateUserRequest dto)
    {
        var changed = false;
        var error = string.Empty;

        if (dto.Email != null && !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.UpdateEmail(dto.Email);
            changed = true;
        }

        if (!string.IsNullOrEmpty(dto.NewPassword) && !string.IsNullOrEmpty(dto.CurrentPassword))
        {
            if (!user.VerifyPassword(dto.CurrentPassword))
                error = "Current password is incorrect.";

            if (user.VerifyPassword(dto.NewPassword))
                error = "New password cannot be the same as the current password.";

            if (string.IsNullOrEmpty(error))
            {
                user.SetPassword(dto.NewPassword);
                user.SetRequiredChangePW(false);
                changed = true;
            }
        }

        if (dto.Detail != null && !string.Equals(user.Detail, dto.Detail, StringComparison.OrdinalIgnoreCase))
        {
            user.UpdateDetail(dto.Detail);
            changed = true;
        }

        if (dto.Status.HasValue && user.Status != (UserStatus)dto.Status.Value)
        {
            user.UpdateStatus((UserStatus)dto.Status.Value);
            changed = true;
        }

        return (changed, error);
    }
}