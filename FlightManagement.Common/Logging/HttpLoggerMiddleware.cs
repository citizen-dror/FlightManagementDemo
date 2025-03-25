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

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString(); // Generate a unique identifier for this request
            _logger.LogInformation("Incoming request {RequestId} at {UtcTime}: {Method} {Path}",
                requestId, DateTime.UtcNow, context.Request.Method, context.Request.Path);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation("Response for request {RequestId} at {UtcTime}: {StatusCode} - {Body}",
                requestId, DateTime.UtcNow, context.Response.StatusCode, responseText);

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
