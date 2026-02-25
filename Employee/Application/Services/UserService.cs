using System.Text.Json.Nodes;
using Employee.Application.DTOs;
using Employee.Application.DTOs.Request;
using Employee.Application.DTOs.Response;
using Employee.Application.Repositories.Queries;
using Employee.Application.Interfaces;
using Employee.Domain.Entities;
using Employee.Domain.Repositories;
using Request.Application.DTOs;
using System.Security.Cryptography;
using Employee.Application.Mapping;
using Employee.Domain.ValueObjects;
using Employee.Application.Extensions;
using System.Text.Json;
using Shared.Abstractions.Paging;

namespace Employee.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IUserQueriesRepository userQueriesRepository,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplateService
) : IUserService
{

    public async Task<PagedResult<GetUserResponse>> GetAllAsync(GetUserRequest? request)
    {
        var data = await userQueriesRepository.GetAll(request);

        var parsedItems = ParseUserListDetails(data.Items);

        return data.WithItems(parsedItems);
    }

    public async Task<GetUserResponse?> GetByIdAsync(int id)
    {
        var data = userQueriesRepository.GetById(id);

        var parsedUser = ParseUserDetail(data);

        return parsedUser;

    }

    public async Task<UserResponse<CreateUserResponse>> CreateAsync(CreateUserRequest dto)
    {
        var randomPassword = GenerateRandomPassword();

        var user = new User(dto.UserName, dto.Email, randomPassword);

        var created = await userRepository.CreateAsync(user);
        if (created == 0)
            return new UserResponse<CreateUserResponse>(false, "Failed to create user.");

        var createdUser = await userRepository.GetByIdAsync(created);
        if (createdUser is null)
            return new UserResponse<CreateUserResponse>(false, "Failed to retrieve created user.");

        var roleAdded = await userRepository.AddRoleToCreatedUser(created);
        if (!roleAdded)
            return new UserResponse<CreateUserResponse>(false, "Failed to assign role to created user.");

        // Send welcome email
        var emailTemplate = emailTemplateService.GetUserCreatedTemplate(dto.UserName, dto.Email, randomPassword);
        await SendEmail(createdUser, MailSubjectType.UserCreated, emailTemplate);

        var response = createdUser.ToViewModel(randomPassword);

        return new UserResponse<CreateUserResponse>(true, "User created successfully.", response);
    }

    public async Task<UserResponse<UpdateUserResponse>?> UpdateAsync(UpdateUserRequest dto)
    {
        var existingUser = await userRepository.GetByIdAsync(dto.UserID);
        if (existingUser == null)
            return new UserResponse<UpdateUserResponse>(false, "User not found.");

        var (detailChanged, mergedDetail) = MergeUserDetail(existingUser, dto);
        if (detailChanged)
            existingUser.UpdateDetail(mergedDetail);

        var (changed, error) = existingUser.ToEntity(dto);
        if (!changed)
            return new UserResponse<UpdateUserResponse>(false, error);

        var updatedUser = await userRepository.UpdateAsync(existingUser);
        if (updatedUser == null)
            return new UserResponse<UpdateUserResponse>(false, "Failed to update user.");

        // Send notification email
        var emailTemplate = emailTemplateService.GetUserUpdatedTemplate(
            updatedUser.UserName,
            updatedUser.Email);
        await SendEmail(updatedUser, MailSubjectType.UserUpdated, emailTemplate);

        var response = updatedUser.ToViewModel();

        return new UserResponse<UpdateUserResponse>(true, "User updated successfully.", response);
    }

    public async Task<UserResponse<bool>> DeleteAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
            return new UserResponse<bool>(false, "User not found.", false);

        var userName = user.UserName;
        var email = user.Email;

        var result = await userRepository.DeleteAsync(id);
        if (!result)
            return new UserResponse<bool>(false, "Failed to delete user.", false);

        // Send account deleted email
        var emailTemplate = emailTemplateService.GetUserDeletedTemplate(userName, email);
        await SendEmail(user, MailSubjectType.UserDeleted, emailTemplate);

        return new UserResponse<bool>(true, "User deleted successfully.");
    }

    public async Task<UserResponse<string>> ResetPasswordAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
            return new UserResponse<string>(false, "User not found.");

        var newPassword = GenerateRandomPassword();
        user.ResetFailedLoginCount();
        user.SetPassword(newPassword);
        user.SetRequiredChangePW(true);

        var updatedUser = await userRepository.UpdateAsync(user);
        if (updatedUser == null)
            return new UserResponse<string>(false, "Failed to reset password.");

        // Send password reset email
        var emailTemplate = emailTemplateService.GetPasswordResetTemplate(user.UserName, user.Email, newPassword);
        await SendEmail(updatedUser, MailSubjectType.PasswordReset, emailTemplate);

        return new UserResponse<string>(true, "Password reset successfully.", newPassword);
    }

    public async Task<UserResponse<bool>> LockUserAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user == null)
            return new UserResponse<bool>(false, "User not found.");

        user.UpdateStatus(UserStatus.Locked);

        var updatedUser = await userRepository.UpdateAsync(user);
        if (updatedUser == null)
            return new UserResponse<bool>(false, "Failed to lock user.");

        // Send account locked email
        var emailTemplate = emailTemplateService.GetAccountLockedTemplate(user.UserName, user.Email, DateTime.Now);
        await SendEmail(updatedUser, MailSubjectType.AccountLocked, emailTemplate);

        return new UserResponse<bool>(true, "User locked successfully.");
    }

    private List<GetUserResponse> ParseUserListDetails(IReadOnlyList<GetUserResponse> users)
    {
        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.Detail)) continue;

            var detailObj = JsonNode.Parse(user.Detail);
            if (detailObj == null) continue;

            user.FirstName = detailObj["FirstName"]?.GetValue<string>();
            user.LastName = detailObj["LastName"]?.GetValue<string>();
            user.PhoneNumber = detailObj["PhoneNumber"]?.GetValue<string>();
            user.BirthDate = detailObj["BirthDate"]?.GetValue<string>();
            user.Address = detailObj["Address"]?.GetValue<string>();
            user.Location = detailObj["Location"]?.GetValue<string>();
            user.TimeZone = detailObj["TimeZone"]?.GetValue<string>();
        }

        return users.ToList();
    }

    private GetUserResponse? ParseUserDetail(GetUserResponse? user)
    {
        if (string.IsNullOrEmpty(user?.Detail)) return user;

        var detailObj = JsonNode.Parse(user.Detail);
        if (detailObj == null) return user;

        user.FirstName = detailObj["FirstName"]?.GetValue<string>();
        user.LastName = detailObj["LastName"]?.GetValue<string>();
        user.PhoneNumber = detailObj["PhoneNumber"]?.GetValue<string>();
        user.BirthDate = detailObj["BirthDate"]?.GetValue<string>();
        user.Address = detailObj["Address"]?.GetValue<string>();
        user.Location = detailObj["Location"]?.GetValue<string>();
        user.TimeZone = detailObj["TimeZone"]?.GetValue<string>();

        return user;
    }

    private string GenerateRandomPassword(int length = 12)
    {
        if (length < 4) length = 4; // ensure room for required classes

        const string lowers = "abcdefghijklmnopqrstuvwxyz";
        const string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string specials = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        var all = lowers + uppers + digits + specials;
        var pwChars = new List<char>(length);

        using var rng = RandomNumberGenerator.Create();

        char GetRandomChar(string set)
        {
            var buf = new byte[4];
            rng.GetBytes(buf);
            var idx = (int)(BitConverter.ToUInt32(buf, 0) % (uint)set.Length);
            return set[idx];
        }

        // ensure at least one of each category
        pwChars.Add(GetRandomChar(lowers));
        pwChars.Add(GetRandomChar(uppers));
        pwChars.Add(GetRandomChar(digits));
        pwChars.Add(GetRandomChar(specials));

        for (int i = pwChars.Count; i < length; i++)
            pwChars.Add(GetRandomChar(all));

        // shuffle
        for (int i = pwChars.Count - 1; i > 0; i--)
        {
            var buf = new byte[4];
            rng.GetBytes(buf);
            int j = (int)(BitConverter.ToUInt32(buf, 0) % (uint)(i + 1));
            (pwChars[i], pwChars[j]) = (pwChars[j], pwChars[i]);
        }

        return new string(pwChars.ToArray());
    }

    private async Task SendEmail(User user, MailSubjectType subjectType, string emailHtml)
    {
        // No reply email address -- system email
        var noReplyEmail = "tai.vv@linhlongengineering.com";

        // Approver email address
        var recipientEmail = new[] { "tai.vv@linhlongengineering.com" };

        // Requester email address as CC
        // var ccEmail = string.IsNullOrEmpty(request.Email)
        //     ? Array.Empty<string>()
        //     : new[] { request.Email };
        var ccEmail = Array.Empty<string>();

        var message = new EmailMessage
        {
            From = noReplyEmail,
            To = recipientEmail,
            Cc = ccEmail,
            Subject = subjectType.ToSubject(user.UserName),
            Html = emailHtml
        };

        await emailSender.SendAsync(message);
    }

    private (bool, string) MergeUserDetail(User user, UpdateUserRequest dto)
    {
        if (string.IsNullOrEmpty(dto.Detail)) return (false, string.Empty);
        Console.WriteLine("Merging user detail:");
        Console.WriteLine(dto.Detail);

        if (string.Equals(user.Detail, dto.Detail, StringComparison.OrdinalIgnoreCase))
            return (false, string.Empty);

        var existingDetail = string.IsNullOrEmpty(user.Detail)
            ? new JsonObject()
            : JsonNode.Parse(user.Detail)!.AsObject();

        var newDetail = JsonNode.Parse(dto.Detail)!.AsObject();

        var mergedDetail = JsonMerge.Merge(existingDetail, newDetail);

        return (true, mergedDetail.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
    }

}