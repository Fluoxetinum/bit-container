using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BitContainer.Shared.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (Int32) HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync((new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.ToString()
            }).ToString());
        }
    }
}
