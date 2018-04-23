using System;
using Kotas.Utils.AspNet.Middlewares.ExceptionToResponse;
using Microsoft.AspNetCore.Builder;

namespace Kotas.Utils.AspNet
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseExceptionToResponseMiddleware(this IApplicationBuilder app)
        {
            var exceptionsMapper = app.ApplicationServices.GetService(typeof(ExceptionToResponseMapper));

            if (exceptionsMapper == null)
            {
                throw new Exception($"Can not resolve {nameof(ExceptionToResponseMapper)} from container. " +
                                    $"Call services.AddExceptionToResponseMiddleware() method to register mapper.");
            }

            app.UseMiddleware<ExceptionToResponseMiddleware>(exceptionsMapper);
        }
    }
}
