using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ECommerceApp.Filters
{
    public class ControllerActionLoggingFilter(ILogger<ControllerActionLoggingFilter> logger) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var actionName = context.ActionDescriptor.DisplayName ?? "UnknownAction";
            var httpContext = context.HttpContext;
            var userId = httpContext.User.FindFirst("customer_id")?.Value;

            logger.LogInformation(
                "Executing action {ActionName} {Method} {Path} for user {UserId}.",
                actionName,
                httpContext.Request.Method,
                httpContext.Request.Path,
                userId ?? "anonymous");

            var executedContext = await next();
            stopwatch.Stop();

            if (executedContext.Exception != null && !executedContext.ExceptionHandled)
            {
                logger.LogError(
                    executedContext.Exception,
                    "Action {ActionName} failed after {ElapsedMilliseconds}ms.",
                    actionName,
                    stopwatch.ElapsedMilliseconds);
                return;
            }

            var statusCode = httpContext.Response.StatusCode;
            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                logger.LogError(
                    "Action {ActionName} returned status {StatusCode} in {ElapsedMilliseconds}ms.",
                    actionName,
                    statusCode,
                    stopwatch.ElapsedMilliseconds);
                return;
            }

            if (statusCode >= StatusCodes.Status400BadRequest)
            {
                logger.LogWarning(
                    "Action {ActionName} returned status {StatusCode} in {ElapsedMilliseconds}ms.",
                    actionName,
                    statusCode,
                    stopwatch.ElapsedMilliseconds);
                return;
            }

            logger.LogInformation(
                "Executed action {ActionName} with status {StatusCode} in {ElapsedMilliseconds}ms.",
                actionName,
                statusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
