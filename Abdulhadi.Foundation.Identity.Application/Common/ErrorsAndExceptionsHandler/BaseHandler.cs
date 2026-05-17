using BuildingBlocks.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Abdulhadi.Foundation.Identity.Application.Common.ErrorsAndExceptionsHandler;

public abstract class BaseHandler<TRequest, TResult>
{
    protected readonly ILogger _logger;

    protected BaseHandler(ILogger logger)
    {
        _logger = logger;
    }

    protected async Task<OutputResult<TResult>> HandleWithErrorHandlingAsync(
        TRequest request,
        string operationName,
        Func<TRequest, Task<OutputResult<TResult>>> action)
    {
        try
        {
            _logger.LogInformation(
                "{Operation} started",
                operationName);

            var result = await action(request);

            _logger.LogInformation(
                "{Operation} completed successfully",
                operationName);

            return result;
        }
        catch (AppException ex)
        {
            var errorMessage = ex.SourceLayer switch
            {
                "Abdulhadi.Foundation.Identity.Api" => ex.Message,
                "Abdulhadi.Foundation.Identity.Domain" => ex.Message,
                "Abdulhadi.Foundation.Identity.Application" => ex.Message,
                "Abdulhadi.Foundation.Identity.Infrastructure" or
                "Abdulhadi.Foundation.Identity.Infrastructure.Persistence" => "A system error occurred",
                _ => "Unexpected error occurred"
            };

            _logger.LogWarning(
                ex,
                "{Operation} failed (AppException) | ErrorCode: {ErrorCode}",
                operationName,
                ex.ErrorCode);

            return OutputResult<TResult>.Fail(errorMessage, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{Operation} failed unexpectedly",
                operationName);

            return OutputResult<TResult>.Fail("An unexpected error occurred");
        }
    }
}