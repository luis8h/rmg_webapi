using System.Net;
using System.Security.Authentication;
using webapi.Errors;

namespace webapi.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(
                RequestDelegate next,
                ILogger<ExceptionMiddleware> logger,
                IHostEnvironment env
                )
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                ApiError response;

                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

                string message;
                var exceptionType = ex.GetType();

                if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    statusCode = HttpStatusCode.Forbidden;
                    message = "You are not authorized";
                } else if (exceptionType == typeof(AuthenticationException))
                {
                    statusCode = HttpStatusCode.Forbidden;
                    message = "Authentication Failed";
                } else {
                    statusCode = HttpStatusCode.InternalServerError;
                    message = ex.Message == null ? "Unknown Error" : ex.Message;
                }

                if (env.IsDevelopment())
                {
                    response = new ApiError((int) statusCode, message, ex.StackTrace!.ToString());
                } else {
                    response = new ApiError((int) statusCode, message);
                }

                logger.LogError(ex, message);

                context.Response.StatusCode = (int) statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(response.ToString());
            }
        }
    }
}
