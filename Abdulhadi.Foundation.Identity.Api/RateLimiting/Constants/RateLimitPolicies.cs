namespace Abdulhadi.Foundation.Identity.Api.RateLimiting.Constants;

public static class RateLimitPolicies
{
    public const string Login_5 = nameof(Login_5);
    public const string ConfirmEmail_3 = nameof(ConfirmEmail_3);
    public const string ResendCode_2 = nameof(ResendCode_2);
    public const string RefreshToken_10 = nameof(RefreshToken_10);
    public const string ForgotPassword_3 = nameof(ForgotPassword_3);
    public const string Default_150 = nameof(Default_150);
    public const string General_40 = nameof(General_40);
    public const string Register_5 = nameof(Register_5);
    public const string PublicApi_60 = nameof(PublicApi_60);
}