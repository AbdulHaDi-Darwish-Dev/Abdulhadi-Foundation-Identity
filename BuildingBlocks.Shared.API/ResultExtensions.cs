using Microsoft.AspNetCore.Mvc;
using BuildingBlocks.Shared.Core;
using BuildingBlocks.Shared.Contracts;

namespace BuildingBlocks.Shared.API
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this OutputResult<T> result)
        {
            // ✅ تحديد StatusCode
            var statusCode = result.Success
                ? result.StatusCode ?? 200
                : ErrorMapper.MapToStatusCode(result.ErrorCode);

            // ✅ إنشاء ApiResponse موحد
            var response = result.Success
                ? ApiResponse<T>.Ok(result.Result)
                : ApiResponse<T>.Fail(result.ErrorMessage ?? "An error occurred", result.ErrorCode);

            return new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }
    }
}