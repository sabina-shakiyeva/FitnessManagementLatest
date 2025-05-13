using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fitness.Business.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Növbəti middleware-ə ötür
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Global Exception baş verdi");

                var statusCode = ex switch
                {
                    InvalidOperationException => HttpStatusCode.BadRequest,
                    ArgumentNullException => HttpStatusCode.BadRequest,
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    _ => HttpStatusCode.InternalServerError
                };

                var response = new
                {
                    status = (int)statusCode,
                    error = ex.Message
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }

    }
}
