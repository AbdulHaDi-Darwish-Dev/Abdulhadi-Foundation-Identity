using BuildingBlocks.Shared.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;
using Abdulhadi.Foundation.Identity.Application.Features.Roles.Specifications;
using Abdulhadi.Foundation.Identity.Application.Features.Users.Specifications;
using Abdulhadi.Foundation.Identity.Application.Common.ErrorsAndExceptionsHandler;

namespace Abdulhadi.Foundation.Identity.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<OutputResult<bool>> RegisterAsync(RegisterRequest request, bool isExternalUser = false)
    {
        return await BaseHandler.HandleWithErrorHandlingAsync(request, "Registration", _logger, async c =>
        {
            try
            {
                _logger.LogInformation("Registration attempt started for username: {Username}", c.Username);

                var userRepo = _unitOfWork.Repository<ApplicationUser>();

                // 1 جلب ريبوزيتوري جدول الأدوار وجدول العلاقة بين المستخدم والدور
                var roleRepo = _unitOfWork.Repository<IdentityRole<Guid>>();
                var userRoleRepo = _unitOfWork.Repository<IdentityUserRole<Guid>>();

                // 2 التحقق من توفر اسم المستخدم باستخدام الـ Specification
                _logger.LogDebug("Checking if username is available: {Username}", c.Username);

                var usernameSpec = new UserByUsernameSpec(c.Username, includeDeleted: true);
                var usernameExists = await userRepo.ExistsAsync(usernameSpec);

                if (usernameExists)
                {
                    _logger.LogWarning("Registration failed: Username already exists - {Username}", c.Username);
                    return OutputResult<bool>.Fail("Username already exists", ErrorCodes.Conflict);
                }

                // 3 التحقق من توفر البريد الإلكتروني
                _logger.LogDebug("Checking if email is available: {Email}", c.Email);

                var emailSpec = new UserByEmailSpec(c.Email, includeDeleted: true);
                var emailExists = await userRepo.ExistsAsync(emailSpec);

                if (emailExists)
                {
                    _logger.LogWarning("Registration failed: Email already exists - {Email}", c.Email);
                    return OutputResult<bool>.Fail("Email already exists", ErrorCodes.Conflict);
                }

                _logger.LogDebug("Username and email validation passed for: {Username}", c.Username);

                // 4 إنشاء المستخدم عبر الـ Domain Factory Method الخاصة بك
                var user = ApplicationUser.Create(c.Email, c.Username, c.Password, isExternalUser);

                // 5 إضافة المستخدم إلى الـ Repository
                await userRepo.AddAsync(user);

                // 6 جلب الدور الافتراضي "User" من قاعدة البيانات (الذي وضعناه في الـ Seed)
                // سنستخدم هنا الـ Specification لجلب الدور باسمه
                var roleSpec = new RoleByNameSpec("User");
                var defaultRole = await roleRepo.FirstOrDefaultAsync(roleSpec);

                if (defaultRole == null)
                {
                    _logger.LogError("Default role 'User' was not found in the database.");
                    return OutputResult<bool>.Fail("System configuration error");
                }

                // 7 ربط المستخدم بالدور الافتراضي في الجدول الوسيط
                var userRoleLink = new IdentityUserRole<Guid>
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                };
                await userRoleRepo.AddAsync(userRoleLink);

                // 8 حفظ كل التغييرات (إضافة المستخدم + ربطه بالدور) في Transaction واحدة
                var commitResult = await _unitOfWork.CommitAsync();
                if (!commitResult.Success)
                {
                    return OutputResult<bool>.Fail("Database save failed");
                }

                _logger.LogInformation("User successfully registered with default 'User' role: {UserId}", user.Id);
                return OutputResult<bool>.Ok(true, StatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration for username: {Username}", c.Username);
                throw;
            }
        });
    }
}