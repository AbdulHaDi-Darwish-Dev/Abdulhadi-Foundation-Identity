using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Shared.API;
using Microsoft.AspNetCore.RateLimiting;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Api.RateLimiting.Constants;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [EnableRateLimiting(RateLimitPolicies.Register_5)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request, isExternalUser: false);

        return result.ToActionResult();
    }
}