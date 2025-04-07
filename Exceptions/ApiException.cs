namespace WebApplication1.Exceptions
{
    public class ApiException(string message, int statusCode)
    {
        public string Message { get; set; } = message;
        public int StatusCode { get; set; } = statusCode;
    }
}
