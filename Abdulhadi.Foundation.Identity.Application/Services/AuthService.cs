using BuildingBlocks.Shared.Core;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Abdulhadi.Foundation.Identity.Domain.Enums;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.DTOs.Response;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Authentication;
using Abdulhadi.Foundation.Identity.Application.Common.ErrorsAndExceptionsHandler;

namespace Abdulhadi.Foundation.Identity.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IJwtProvider _jwtProvider;

    private readonly ILogger<AuthService> _logger;

    private readonly ISecurityCodeService _securityCodeService;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(
        IJwtProvider jwtProvider,
        ILogger<AuthService> logger,
        ISecurityCodeService securityCodeService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _jwtProvider = jwtProvider;
        _userManager = userManager;
        _signInManager = signInManager;
        _securityCodeService = securityCodeService;
    }

    public async Task<OutputResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        return await BaseHandler.HandleWithErrorHandlingAsync(request, "Login", _logger, async c =>
        {
            _logger.LogInformation("Login attempt started for identifier: {Identifier}", c.Identifier);

            // 1. محاولة البحث بالإيميل أولاً
            var user = await _userManager.FindByEmailAsync(c.Identifier)
                        ?? await _userManager.FindByNameAsync(c.Identifier); // إن لم يجد، يبحث بالـ Username

            if (user is null)
            {
                _logger.LogWarning("Login failed: User with identifier {Identifier} not found.", c.Identifier);

                return OutputResult<AuthResponse>.Fail("Invalid credentials", ErrorCodes.InvalidCredentials);
            }

            // 2. الفحص أولاً: هل الحساب مقفل حالياً بناءً على الـ 5 دقائق التي حددتها؟
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Login rejected: Account is locked out for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail("Account is temporarily locked. Please try again later.", ErrorCodes.Forbidden);
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login rejected: Account is disabled for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail("Account is disabled. Please contact support.", ErrorCodes.Forbidden);
            }

            // 3. التحقق من كلمة المرور وتفعيل عداد المحاولات (lockoutOnFailure: true)
            // هنا سيقوم النظام تلقائياً بقراءة الـ MaxFailedAccessAttempts = 5 التي وضعتها في الـ Program.cs
            var result = await _signInManager.CheckPasswordSignInAsync(user, c.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                _logger.LogWarning(
                    "Login failed: User {UserId} reached 5 failed attempts and is now locked out for 5 minutes.",
                    user.Id);

                return OutputResult<AuthResponse>.Fail("Account is temporarily locked due to multiple failed attempts.", ErrorCodes.Forbidden);
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Incorrect password for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail("Invalid credentials", ErrorCodes.InvalidCredentials);
            }

            // 4. التحقق من تفعيل البريد (حسب منطق البزنس الخاص بك)
            if (!user.EmailConfirmed)
            {
                _logger.LogInformation("Login suspended: Email verification required for user {UserId}.", user.Id);

                await _securityCodeService.SendOtpAsync(user, OtpType.EmailVerification);

                return OutputResult<AuthResponse>.Ok(new AuthResponse
                {
                    RequiresVerification = true,
                    Message = "Email not verified. Verification code sent."
                });
            }

            _logger.LogInformation("Credentials verified. Generating tokens for user {UserId}...", user.Id);

            // Access Token & Refresh Token Generation
            var accessToken = await _jwtProvider.GenerateAccessTokenAsync(user);

            var refreshTokenValue = _jwtProvider.GenerateRefreshToken();
            var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue, expiryDays: 7);

            user.RefreshTokens.Add(refreshToken);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Login failed: Unable to update user tokens for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail("An error occurred during login processing.");
            }

            _logger.LogInformation("User {UserId} successfully logged in.", user.Id);

            return OutputResult<AuthResponse>.Ok(new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),

                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            });
        });
    }

    public async Task<OutputResult<string>> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        return await BaseHandler.HandleWithErrorHandlingAsync(request, "ConfirmEmail", _logger, async c =>
        {
            _logger.LogInformation("Confirm email attempt started for email: {Email}", c.Email);

            // 1. جلب المستخدم من قاعدة البيانات
            var user = await _userManager.FindByEmailAsync(c.Email);
            if (user is null)
            {
                _logger.LogWarning("Confirm email failed: User with email {Email} not found.", c.Email);

                return OutputResult<string>.Fail("User not found.", ErrorCodes.NotFound);
            }

            // 2. إذا كان البريد مؤكداً بالفعل، لا داعي لإعادة العملية
            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Confirm email skipped: Email already confirmed for user {UserId}.", user.Id);

                return OutputResult<string>.Ok("Email is already confirmed.");
            }

            // 3. التحقق من الـ OTP عبر الـ SecurityCodeService
            var isValidOtp = await _securityCodeService.VerifyOtpAsync(c.Email, c.Code, OtpType.EmailVerification);

            if (!isValidOtp)
            {
                _logger.LogWarning("Confirm email failed: Invalid or expired OTP code for user {UserId}.", user.Id);

                return OutputResult<string>.Fail("Invalid or expired verification code.", ErrorCodes.VerificationFailed);
            }

            // 5. تحديث حالة تأكيد الإيميل في قاعدة البيانات عبر Identity
            user.ConfirmEmail();

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogError("Confirm email failed: Unable to update user confirmation status in database for user {UserId}.", user.Id);

                return OutputResult<string>.Fail("An error occurred while confirming your email.");
            }

            _logger.LogInformation("Email successfully confirmed for user {UserId}.", user.Id);

            return OutputResult<string>.Ok("Email confirmed successfully.");
        });
    }
}