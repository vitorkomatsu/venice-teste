namespace Venice.Teste.Backend.WebApi.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Venice.Teste.Backend.Domain.Exceptions;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Ocorreu um erro não tratado.");

            switch (exception)
            {
                case KeyNotFoundException:
                    context.Response.StatusCode = 404;
                    break;
                case ApiException:
                    context.Response.StatusCode = 400;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            var result = new
            {
                message = exception.Message,
                statusCode = context.Response.StatusCode
            };

            return context.Response.WriteAsJsonAsync(result);
        }
    }
}