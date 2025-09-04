using System.Diagnostics;
using Serilog;

namespace Ecommerce.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var requestHeaders = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var ip = context.Connection.RemoteIpAddress?.ToString();


            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);


            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            stopwatch.Stop();

            Log.Information("API Hit Log\n" +
                            "Timestamp: {Timestamp}\n" +
                            "IP Address: {IpAddress}\n" +
                            "Method: {Method}\n" +
                            "URL: {Url}\n" +
                            "Request Headers: {RequestHeaders}\n" +
                            "Request Body: {RequestBody}\n" +
                            "Status Code: {StatusCode}\n" +
                            "Response Body: {ResponseBody}\n" +
                            "Execution Time: {ExecutionTime} sec\n",
                            DateTime.UtcNow,
                            ip,
                            context.Request.Method,
                            context.Request.Path,
                            requestHeaders,
                            requestBody,
                            context.Response.StatusCode,
                            responseBodyText,
                            stopwatch.Elapsed.TotalSeconds
                        );


            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
