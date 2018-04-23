using Kotas.Utils.Common;
using Kotas.Utils.Data.EntityFramework.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;
using Kotas.Utils.Data.UnitOfWork.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kotas.Utils.Data.EntityFramework
{
    public static class Bootstrap
    {
        private static string _defaultConnectionString = "default";

        public static void SetDefaultConnectionString(string connectionString)
        {
            Validations.ValidateInput(connectionString, nameof(connectionString));

            _defaultConnectionString = connectionString;
        }

        public static void AddEntityFrameworkDataAccess<TContext>(this IServiceCollection services, IConfigurationRoot configuration) 
            where TContext: DbContext
        {
            Validations.ValidateInput(services, nameof(IServiceCollection));
            Validations.ValidateInput(configuration, nameof(IConfigurationRoot));

            var registry = new AsyncLocalUnitOfWorkRegistry();
            services.AddSingleton<IUnitOfWorkProvider>(app => new EntityFrameworkUnitOfWorkProvider(registry, 
                (connectionString, type) => app.GetService<TContext>()));
        }

        public static void AddEntityFrameworkDataAccessWithDatabase<TContext>(this IServiceCollection services, IConfigurationRoot configuration, 
            DatabaseType database, string connectionString) where TContext: DbContext
        {
            Validations.ValidateInput(services, nameof(IServiceCollection));
            Validations.ValidateInput(configuration, nameof(IConfigurationRoot));

            connectionString = string.IsNullOrWhiteSpace(connectionString) ? _defaultConnectionString : connectionString;
            var isJustName = !connectionString.Contains(" ");

            if (isJustName)
            {
                connectionString = configuration.GetConnectionString(connectionString);
            }

            switch (database)
            {
                case DatabaseType.PostgreSql:
                    services
                        .AddEntityFrameworkNpgsql()
                        .AddDbContext<TContext>(options => options.UseNpgsql(connectionString));
                    break;
                default:
                    services
                        .AddEntityFrameworkSqlServer()
                        .AddDbContext<TContext>(options => options.UseSqlServer(connectionString));
                    break;
            }

            var registry = new AsyncLocalUnitOfWorkRegistry();
            services.AddSingleton<IUnitOfWorkProvider>(app => new EntityFrameworkUnitOfWorkProvider(registry, 
                (conn, type) => app.GetService<TContext>()));
        }
    }
}
