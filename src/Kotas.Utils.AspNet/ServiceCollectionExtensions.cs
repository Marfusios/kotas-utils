using System;
using System.Collections.Generic;
using System.Net;
using Kotas.Utils.AspNet.Middlewares.ExceptionToResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kotas.Utils.AspNet
{
    public static class ServiceCollectionExtensions
    {
        public static void AddExceptionToResponseMiddleware(this IServiceCollection services, 
            IDictionary<Type, HttpStatusCode> mapping = null)
        {
            var mapper = new ExceptionMapping();
            mapper.RegisterMapping(mapping);

            services.TryAddSingleton(mapper);
            services.TryAddSingleton<ExceptionToResponseMapper>();
        }
    }
}
