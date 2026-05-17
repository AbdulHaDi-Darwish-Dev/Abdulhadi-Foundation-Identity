using BuildingBlocks.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Abdulhadi.Foundation.Identity.Application.Common.ErrorsAndExceptionsHandler;

public static class BaseHandler
{
    // تم إضافة <TRequest, TResult> وتم تمرير الـ ILogger كمُعامل
    public static async Task<OutputResult<TResult>> HandleWithErrorHandlingAsync<TRequest, TResult>(
        TRequest request,
        string operationName,
        ILogger logger, // حقن اللوجر هنا لحل مشكلة الـ Thread Safety
        Func<TRequest, Task<OutputResult<TResult>>> action)
    {
        try
        {
            logger.LogInformation("{Operation} started", operationName);

            var result = await action(request);

            logger.LogInformation("{Operation} completed successfully", operationName);

            return result;
        }

        catch (AppException ex)
        {
            // تحسين طريقة فحص الـ Layer لتكون أكثر مرونة (تحتوي على النص بدل التطابق التام)
            string errorMessage = ex.SourceLayer switch
            {
                var layer when layer.Contains(".Api") => ex.Message,
                var layer when layer.Contains(".Domain") => ex.Message,
                var layer when layer.Contains(".Application") => ex.Message,
                var layer when layer.Contains(".Infrastructure") => "A system error occurred",
                _ => "Unexpected error occurred"
            };

            logger.LogWarning(
                ex,
                "{Operation} failed (AppException) | ErrorCode: {ErrorCode}",
                operationName,
                ex.ErrorCode);

            return OutputResult<TResult>.Fail(errorMessage, ex.ErrorCode);
        }

        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{Operation} failed unexpectedly",
                operationName);

            return OutputResult<TResult>.Fail("An unexpected error occurred");
        }
    }
}