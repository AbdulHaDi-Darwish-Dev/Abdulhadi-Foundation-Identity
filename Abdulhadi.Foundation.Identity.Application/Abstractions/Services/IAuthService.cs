using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.DTOs.Response;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

public interface IAuthService
{
    Task<OutputResult<AuthResponse>> LoginAsync(LoginRequest request);

    Task<OutputResult<string>> ConfirmEmailAsync(ConfirmEmailRequest request);

    Task<OutputResult<string>> ResendVerificationCodeAsync(ResendCodeRequest request);
}