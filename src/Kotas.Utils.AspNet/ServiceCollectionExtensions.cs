using Kotas.Utils.AspNet.Middlewares.ExceptionToResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kotas.Utils.AspNet
{
    public static class ServiceCollectionExtensions
    {
        public static void AddExceptionToResponseMiddleware(this IServiceCollection services)
        {
            services.TryAddSingleton<ExceptionToResponseMapper>();
        }
    }
}
