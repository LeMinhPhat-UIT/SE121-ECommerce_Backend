using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Commons;
using System.Text.RegularExpressions;

namespace ECommerceApp.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var (statusCode, errorType, englishMessage) = MapException(ex);

                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    _logger.LogError(ex, "Backend Error occurred while processing request {Path}", httpContext.Request.Path);
                }
                else
                {
                    _logger.LogWarning(ex, "{ErrorType} occurred while processing request {Path}: {Message}", errorType, httpContext.Request.Path, ex.Message);
                }

                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the global exception middleware will not write an error response");
                    throw;
                }

                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";

                var errors = new List<string>
                {
                    englishMessage,
                    $"Type: {errorType}",
                    $"Original message: {GetDeepestMessage(ex)}"
                };

                if (_environment.IsDevelopment())
                {
                    errors.Add($"Detail: Error: {GetDeepestMessage(ex)} | Location: {GetErrorLocation(ex)}");
                }

                var response = new ApiResponse<object>(statusCode, errors);

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }

        private static (int StatusCode, string ErrorType, string EnglishMessage) MapException(Exception ex)
        {
            return ex switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Frontend Error / Invalid Input", "Invalid arguments provided. Please check your input data."),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "User Error / Invalid Operation", "The requested operation is invalid in the current state."),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "User Error / Unauthorized", "You do not have permission to access this resource."),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "User Error / Not Found", "The requested resource was not found."),
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Backend Error / Database Conflict", "Data has been modified by another process. Please try again."),
                DbUpdateException => (StatusCodes.Status400BadRequest, "Backend Error / Database Error", "Database operation failed, possibly due to a constraint violation."),
                NotImplementedException => (StatusCodes.Status501NotImplemented, "Backend Error / Not Implemented", "This feature is not yet implemented by the backend."),
                _ => (StatusCodes.Status500InternalServerError, "Backend Error / Internal Server Error", "An unexpected internal server error occurred.")
            };
        }

        private static string GetDeepestMessage(Exception ex)
        {
            var current = ex;

            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return current.Message;
        }

        private static string GetErrorLocation(Exception ex)
        {
            if (string.IsNullOrEmpty(ex.StackTrace))
            {
                return "Unknown location";
            }

            var lines = ex.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var relevantLine = lines.FirstOrDefault(line => line.Contains("ECommerceApp"))?.Trim();

            if (relevantLine != null)
            {
                var match = Regex.Match(relevantLine, @" in (?<location>.*\.cs:line \d+)");
                return match.Success ? match.Groups["location"].Value : relevantLine;
            }

            return "Unknown location";
        }

    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
