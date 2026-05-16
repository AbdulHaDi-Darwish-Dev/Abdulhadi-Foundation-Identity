namespace BuildingBlocks.Shared.Core
{
    public sealed class OutputResult<T>
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? ErrorCode { get; private set; }
        public T? Result { get; private set; }
        public int? StatusCode { get; private set; }

        private OutputResult() { }

        public static OutputResult<T> Ok(T value, int statusCode = 200)
            => new OutputResult<T>
            {
                Success = true,
                Result = value,
                StatusCode = statusCode
            };

        public static OutputResult<T> Fail(string message, string? errorCode = null)
            => new OutputResult<T>
            {
                Success = false,
                ErrorMessage = message,
                ErrorCode = errorCode ,
            };
    }
}