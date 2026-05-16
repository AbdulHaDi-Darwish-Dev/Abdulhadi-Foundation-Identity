namespace BuildingBlocks.Identity.Options;

public class GoogleOptions
{
    // نستخدم هذا الاسم كمفتاح في ملف appsettings.json
    public const string SectionName = "Authentication:Google";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}