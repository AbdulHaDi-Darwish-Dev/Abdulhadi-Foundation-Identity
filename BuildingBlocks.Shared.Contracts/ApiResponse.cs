namespace BuildingBlocks.Shared.Contracts
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public ApiErrorResponse? Error { get; init; }

        public static ApiResponse<T> Ok(T data)
            => new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Error = null
            };

        public static ApiResponse<T> Fail(string message, string? code = null)
            => new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Error = new ApiErrorResponse
                {
                    Code = code,
                    Message = message
                }
            };
    }
}