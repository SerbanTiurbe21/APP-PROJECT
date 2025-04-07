namespace WebApplication1.Middleware
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<LoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.Now;
            var request = context.Request;
            var logMessage = $"[{DateTime.UtcNow}] Request: {request.Method} {request.Path}";

            try
            {
                await _next(context);
                var response = context.Response;
                logMessage += $" | Response: {response.StatusCode} ";
            }
            catch (Exception e)
            {
                logMessage += $" | Exception: {e.Message}";
            }
            finally
            {
                logMessage += $" | Duration: {(DateTime.Now - startTime).TotalMilliseconds} ms";
                _logger.LogInformation(logMessage);
            }
        }
    }
}
