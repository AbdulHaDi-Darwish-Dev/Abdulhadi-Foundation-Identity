using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Shared.API;
using Microsoft.AspNetCore.RateLimiting;
using Abdulhadi.Foundation.Identity.Application.DTOs.Request;
using Abdulhadi.Foundation.Identity.Api.RateLimiting.Constants;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Api.Controllers;


[ApiController]
[Route("api/v1/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.Login_5)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return result.ToActionResult();
    }

    [HttpPost("email/verify/otp")]
    [EnableRateLimiting(RateLimitPolicies.ConfirmEmail_3)]
    public async Task<IActionResult> VerifyEmailByOtp([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.ToActionResult();
    }
}