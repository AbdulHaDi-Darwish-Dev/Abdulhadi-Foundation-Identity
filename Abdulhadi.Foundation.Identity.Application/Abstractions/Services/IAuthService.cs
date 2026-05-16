using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Application.DTOs.Response;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

public interface IAuthService
{
    Task<OutputResult<LoginResponse>> LoginAsync(LoginRequest request);
}