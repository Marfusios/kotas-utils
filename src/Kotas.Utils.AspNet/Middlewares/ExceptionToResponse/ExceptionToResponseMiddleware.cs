using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kotas.Utils.AspNet.Middlewares.ExceptionToResponse
{
    public class ExceptionToResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionToResponseMapper _exceptionsMapper;

        public ExceptionToResponseMiddleware(RequestDelegate next, ExceptionToResponseMapper exceptionsMapper)
        {
            _next = next;
            _exceptionsMapper = exceptionsMapper;
        }

        // ReSharper disable once ConsiderUsingAsyncSuffix
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.OK
                    && string.IsNullOrEmpty(context.Response.ContentType))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EXCEPTION] {ex}");
                var mappedObjectResult = _exceptionsMapper.Map(ex);
                context.Response.StatusCode = mappedObjectResult?.StatusCode
                                              ?? (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(mappedObjectResult?.Value?.ToString());
            }
        }
    }
}
