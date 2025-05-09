using System.Net;

namespace ChatService.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            //dynamic can change its type at runtime
            dynamic response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error, Please try again!"
            };

            if (_env.IsDevelopment())
            {

                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                    Detailed = ex.StackTrace
                };
            }

            await context.Response.WriteAsJsonAsync((object)response);
        }
    }
}
