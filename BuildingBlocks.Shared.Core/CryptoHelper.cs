using System.Text;
using System.Security.Cryptography;

namespace BuildingBlocks.Shared.Core;

public static class CryptoHelper
{
    /// <summary>
    /// توليد نص عشوائي آمن تشفيرياً 
    /// </summary>
    public static string GenerateSecureRandomString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// تحويل أي نص صريح إلى هاش SHA256 مشفرة بقاعدة Base64
    /// </summary>
    public static string HashText(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        var inputBytes = Encoding.UTF8.GetBytes(plainText);
        var hashBytes = SHA256.HashData(inputBytes); // .NET 6+ ميزة سريعة وآمنة لإدارة الذاكرة

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// مطابقة نص صريح مع هاش مخزن للتأكد من صحته
    /// </summary>
    public static bool VerifyHash(string plainText, string hashedText)
    {
        var incomingHash = HashText(plainText);

        // استخدام المقارنة العادية أو استخدام ممارسة حماية ضد الـ Timing Attacks
        return string.Equals(incomingHash, hashedText, StringComparison.Ordinal);
    }
}