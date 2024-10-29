using System.Net;
using System.Text.Json;

namespace Smokeball.Api.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError($"Something went wrong in processing your request: {exception}");

            context.Response.StatusCode = exception switch
            {
                //Placeholder for custome exception
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message
            };

            return context.Response.WriteAsync(errorResponse.ToString());
        }
    }

    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

}
