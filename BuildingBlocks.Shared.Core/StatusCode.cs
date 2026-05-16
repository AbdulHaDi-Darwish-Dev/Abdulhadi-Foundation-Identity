namespace BuildingBlocks.Shared.Core
{
    public static class StatusCode
    {
        // ✅ Success
        public const int OK = 200;                 // طلب ناجح (GET, PUT, PATCH)
        public const int Created = 201;            // تم إنشاء مورد جديد (POST)
        public const int Accepted = 202;           // الطلب مقبول لكن لم يُنفذ بعد (Async processing)
        public const int NoContent = 204;          // نجاح بدون محتوى (DELETE غالبًا)
    }
}