using System.Net;
using System.Text.Json;

namespace WebApplication1.Exceptions
{
    // it sits in the request pipelines
    // RequestDelegate next is the next thing in the pipeline - the Controller
    public class ExceptionMiddleware(RequestDelegate next)
    {
        // save the middleware into a private field so that I can be used later on
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // move to the next stage (controllers)
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // catch and handle the exception
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set the Response to JSON
            context.Response.ContentType = "application/json";

            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ProductNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case DuplicateProductException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Internal Server Error.";
                    break;
            }

            // Set the HTTP Status Code
            context.Response.StatusCode = (int)status;
            // Serialize the Error as JSON
            var result = JsonSerializer.Serialize(new ApiException(message, (int)status));

            // Write the JSON to the Response
            return context.Response.WriteAsync(result);
        }
    }
}
