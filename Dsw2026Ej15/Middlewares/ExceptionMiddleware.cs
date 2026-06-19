using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Dsw2026Ej15.Api.Exceptions; 

namespace Dsw2026Ej15.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                
                await _next(context);
            }
            catch (ValidationException ex)
            {
                
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception)
            {
                
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";

                
                await context.Response.WriteAsync("Ocurrió un problema en el servidor.");
            }
        }
    }
}