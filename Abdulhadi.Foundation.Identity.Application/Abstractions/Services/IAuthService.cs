using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.DTOs.Response;
using Abdulhadi.Foundation.Identity.Domain.Enums;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

public interface IAuthService
{
    Task<OutputResult<AuthResponse>> LoginAsync(LoginRequest request);
}