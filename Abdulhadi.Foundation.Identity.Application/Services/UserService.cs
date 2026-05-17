using BuildingBlocks.Shared.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;
using Abdulhadi.Foundation.Identity.Application.Common.ErrorsAndExceptionsHandler;

namespace Abdulhadi.Foundation.Identity.Application.Services;

public sealed class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;

    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ILogger<UserService> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<OutputResult<bool>> RegisterAsync(RegisterRequest request, bool isExternalUser = false)
    {
        return await BaseHandler.HandleWithErrorHandlingAsync(request, "Registration", _logger, async c =>
        {
            _logger.LogInformation("Registration attempt started for username: {Username}", c.Username);

            // التحقق من توفر اسم المستخدم
            var usernameExists = await _userManager.Users
                .AnyAsync(x => x.UserName == c.Username);

            if (usernameExists)
            {
                _logger.LogWarning("Registration failed: Username already exists - {Username}", c.Username);

                return OutputResult<bool>.Fail("Username already exists", ErrorCodes.Conflict);
            }

            // التحقق من توفر البريد الإلكتروني
            var emailExists = await _userManager.Users
                .AnyAsync(x => x.Email == c.Email);

            if (emailExists)
            {
                _logger.LogWarning("Registration failed: Email already exists - {Email}", c.Email);

                return OutputResult<bool>.Fail("Email already exists", ErrorCodes.Conflict);
            }

            _logger.LogDebug("Username and email validation passed for: {Username}", c.Username);

            // إنشاء المستخدم
            var user = ApplicationUser.Create(c.Email, c.Username, isExternalUser);

            // إنشاء المستخدم عبر Identity
            var createResult = await _userManager.CreateAsync(user, c.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(x => x.Description));

                _logger.LogWarning("Registration failed for {Username}. Errors: {Errors}", c.Username, errors);

                return OutputResult<bool>.Fail("Unable to create user");
            }

            // إضافة الدور الافتراضي
            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(x => x.Description));

                _logger.LogError("Failed to assign role to user {UserId}. Errors: {Errors}", user.Id, errors);

                return OutputResult<bool>.Fail("User created but role assignment failed");
            }

            _logger.LogInformation("User successfully registered with default 'User' role: {UserId}", user.Id);

            return OutputResult<bool>.Ok(true, StatusCode.Created);
        });
    }
}