namespace BuildingBlocks.Shared.Core
{
    public static class ValidationPatterns
    {
        public const string StrongPassword =
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    }
}