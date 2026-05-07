using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerceApp.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var stopwatch = Stopwatch.StartNew();
            var startedAtUtc = DateTime.UtcNow;

            _logger.LogInformation(
                "Incoming request {Method} {Path}{QueryString} TraceId={TraceId} UserId={UserId} RemoteIp={RemoteIp}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Request.QueryString,
                httpContext.TraceIdentifier,
                httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Method} {Path} TraceId={TraceId}", httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = httpContext.Response.StatusCode;

                _logger.Log(
                    statusCode >= StatusCodes.Status500InternalServerError ? LogLevel.Error :
                    statusCode >= StatusCodes.Status400BadRequest ? LogLevel.Warning : LogLevel.Information,
                    "Outgoing response {StatusCode} for {Method} {Path} completed in {ElapsedMilliseconds}ms at {StartedAtUtc} TraceId={TraceId}",
                    statusCode,
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    startedAtUtc,
                    httpContext.TraceIdentifier);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
