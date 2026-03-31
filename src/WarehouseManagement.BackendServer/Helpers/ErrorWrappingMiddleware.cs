using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WarehouseManagement.BackendServer.Helpers
{
    public class ErrorWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorWrappingMiddleware> _logger;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);

                await HandleExceptionAsync(context, ex);
            }

            if (!context.Response.HasStarted &&
                context.Response.StatusCode >= 400 &&
                context.Response.StatusCode != 204)
            {
                context.Response.ContentType = "application/json";

                var response = new ApiResponse(context.Response.StatusCode);
                var json = JsonConvert.SerializeObject(response);

                await context.Response.WriteAsync(json);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = StatusCodes.Status500InternalServerError;

            var response = new ApiResponse(statusCode, exception.Message);
            var jsonResponse = JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}