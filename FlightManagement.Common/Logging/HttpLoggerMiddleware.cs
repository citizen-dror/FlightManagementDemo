using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Common.Logging
{
    public class HttpLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpLoggerMiddleware> _logger;

        public HttpLoggerMiddleware(RequestDelegate next, ILogger<HttpLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestBody = await ReadRequestBodyAsync(context.Request);

            await _next(context); // Process request

            stopwatch.Stop();
            var responseStatusCode = context.Response.StatusCode;

            _logger.LogInformation(
                "HTTP {Method} {Path} - Request: {RequestBody} - Response: {StatusCode} - Time: {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                requestBody,
                responseStatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            if (request.ContentLength == null || request.ContentLength == 0)
                return string.Empty;

            request.Body.Position = 0; // Reset position
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Reset again for next middleware
            return body;
        }
    }

}
