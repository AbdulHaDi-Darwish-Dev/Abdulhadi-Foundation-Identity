using System.Security.Claims;
using BuildingBlocks.Shared.Core;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Abdulhadi.Foundation.Identity.Domain.Enums;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.DTOs.Response;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Authentication;
using Abdulhadi.Foundation.Identity.Application.Common.ErrorsAndExceptionsHandler;
using Abdulhadi.Foundation.Identity.Application.Features.RefreshTokens.Specifications;

namespace Abdulhadi.Foundation.Identity.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IJwtProvider _jwtProvider;

    private readonly ILogger<AuthService> _logger;

    private readonly ISecurityCodeService _securityCodeService;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtProvider jwtProvider,
        ILogger<AuthService> logger,
        ISecurityCodeService securityCodeService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
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

            // 1. جلب المستخدم
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == c.Identifier || u.UserName == c.Identifier);

            if (user is null)
            {
                _logger.LogWarning("Login failed: User with identifier {Identifier} not found.", c.Identifier);

                return OutputResult<AuthResponse>.Fail("Invalid credentials", ErrorCodes.InvalidCredentials);
            }

            // 2. التحقق من القفل
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Login rejected: Account is locked out for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail(
                    "Account is temporarily locked. Please try again later.",
                    ErrorCodes.Forbidden);
            }

            // 3. التحقق من الحالة
            if (!user.IsActive)
            {
                _logger.LogWarning("Login rejected: Account is disabled for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail(
                    "Account is disabled. Please contact support.",
                    ErrorCodes.Forbidden);
            }

            // 4. التحقق من كلمة المرور
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                c.Password,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login failed: User {UserId} is locked out.", user.Id);

                return OutputResult<AuthResponse>.Fail(
                    "Account is temporarily locked due to multiple failed attempts.",
                    ErrorCodes.Forbidden);
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: Incorrect password for user {UserId}.", user.Id);

                return OutputResult<AuthResponse>.Fail(
                    "Invalid credentials",
                    ErrorCodes.InvalidCredentials);
            }

            // 5. التحقق من البريد
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

            // 6. توليد التوكنز
            var accessToken = await _jwtProvider.GenerateAccessTokenAsync(user);
            var refreshToken = _jwtProvider.CreateRefreshToken(user.Id);

            // 7. حفظ Refresh Token عبر Repository + UnitOfWork فقط
            var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();

            await refreshTokenRepo.AddAsync(refreshToken.RefreshToken);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully logged in.", user.Id);

            // 8. الاستجابة
            return OutputResult<AuthResponse>.Ok(new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),

                RefreshToken = refreshToken.RawToken,
                RefreshTokenExpiresAt = refreshToken.RefreshToken.ExpiresAt
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

    public async Task<OutputResult<string>> ResendVerificationCodeAsync(ResendCodeRequest request)
    {
        return await BaseHandler.HandleWithErrorHandlingAsync(request, "ResendVerificationCode", _logger, async c =>
        {
            _logger.LogInformation("Resend verification code code started for email: {Email}", c.Email);

            // 1. البحث عن المستخدم 
            var user = await _userManager.FindByEmailAsync(c.Email);

            if (user is null)
            {
                // أمنياً: لا نخبر المخترق هل الإيميل موجود أم لا، نرجع رسالة عامة أو نログ الفشل
                _logger.LogWarning("Resend code failed: User with email {Email} not found.", c.Email);

                return OutputResult<string>.Fail("User not found or invalid request.", ErrorCodes.NotFound);
            }

            // 2. التحقق مما إذا كان الحساب مفعلاً بالفعل
            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Resend code skipped: Email is already confirmed for user {UserId}.", user.Id);

                return OutputResult<string>.Fail("Email is already confirmed.", ErrorCodes.VerificationFailed);
            }

            // 3. التحقق مما إذا كان الحساب معطلاً من الإدارة
            if (!user.IsActive)
            {
                _logger.LogWarning("Resend code rejected: Account is disabled for user {UserId}.", user.Id);

                return OutputResult<string>.Fail("Account is disabled.", ErrorCodes.Forbidden);
            }

            // 4. إرسال كود جديد 
            // الـ SecurityCodeService تتكفل بالـ Rate Limiting (دقيقتين) والـ Hashing والتخزين تلقائياً
            _logger.LogInformation("Sending new OTP verification code to user {UserId}...", user.Id);

            await _securityCodeService.SendOtpAsync(user, OtpType.EmailVerification);

            _logger.LogInformation("New OTP verification code sent successfully to user {UserId}.", user.Id);

            return OutputResult<string>.Ok("A new verification code has been sent to your email.");
        });
    }

    public async Task<OutputResult<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        return await BaseHandler.HandleWithErrorHandlingAsync(request, "RefreshToken", _logger, async c =>
        {
            _logger.LogInformation("Refresh token attempt started.");

            // 1. استخراج الـ Claims من Access Token
            var principal = _jwtProvider.GetPrincipalFromExpiredToken(c.AccessToken);

            if (principal is null)
            {
                _logger.LogWarning("Refresh token failed: Invalid access token.");
                return OutputResult<RefreshTokenResponse>.Fail("Invalid tokens.", ErrorCodes.ValidationError);
            }

            // 2. استخراج UserId
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Refresh token failed: Invalid user id claim.");

                return OutputResult<RefreshTokenResponse>.Fail("Invalid tokens.", ErrorCodes.ValidationError);
            }

            // 3. جلب المستخدم فقط للتحقق من الحالة
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null || !user.IsActive)
            {
                _logger.LogWarning("Refresh token failed: User not found or inactive.");

                return OutputResult<RefreshTokenResponse>.Fail("User unavailable.", ErrorCodes.NotFound);
            }

            // 4. البحث عن refresh token في Repository (بدل navigation)
            var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();

            var incomingTokenHash = CryptoHelper.HashText(c.RefreshToken);

            var existingToken = await refreshTokenRepo
                .FirstOrDefaultAsync(new TokenByValueSpec(incomingTokenHash));

            if (existingToken is null || existingToken.UserId != userId)
            {
                _logger.LogWarning("Refresh token failed: Token invalid for user {UserId}.", userId);

                return OutputResult<RefreshTokenResponse>.Fail("Invalid refresh token.", ErrorCodes.ValidationError);
            }

            // 5. التحقق من الصلاحية
            if (!existingToken.IsActive)
            {
                _logger.LogWarning("Refresh token failed: Token expired or revoked.");

                // 🔥 Security: revoke all tokens for this user
                var allTokens = await refreshTokenRepo
                    .ListAsync(new TokenByUserIdSpec(userId));

                foreach (var token in allTokens)
                    token.Revoke();

                await _unitOfWork.SaveChangesAsync();

                return OutputResult<RefreshTokenResponse>.Fail(
                    "Session expired. Please login again.",
                    ErrorCodes.Unauthorized);
            }

            _logger.LogInformation("Refreshing tokens for user {UserId}...", userId);

            // 6. revoke old token
            existingToken.Revoke();

            // 7. generate new tokens
            var newAccessToken = await _jwtProvider.GenerateAccessTokenAsync(user);
            var newRefreshToken = _jwtProvider.CreateRefreshToken(userId);

            await refreshTokenRepo.AddAsync(newRefreshToken.RefreshToken);

            // 8. cleanup expired tokens (optional)
            var expiredTokens = await refreshTokenRepo
                .ListAsync(new TokenExpiredByUserIdSpec(userId));

            _logger.LogInformation("Number of expired tokens found: {Count}", expiredTokens.Count);

            refreshTokenRepo.RemoveRange(expiredTokens);

            // 9. commit everything
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Tokens rotated successfully for user {UserId}.", userId);

            return OutputResult<RefreshTokenResponse>.Ok(new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),

                RefreshToken = newRefreshToken.RawToken,
                RefreshTokenExpiresAt = newRefreshToken.RefreshToken.ExpiresAt
            });
        });
    }
}