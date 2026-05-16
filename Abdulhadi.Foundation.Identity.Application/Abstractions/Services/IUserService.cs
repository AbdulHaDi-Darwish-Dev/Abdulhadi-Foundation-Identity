using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

public interface IUserService
{
    Task<OutputResult<bool>> RegisterAsync(RegisterRequest request);
}