using System.Data;
using System.Data.SqlClient;
using Kotas.Utils.Common;
using Kotas.Utils.Data.Native.Query;
using Kotas.Utils.Data.Native.UnitOfWork.Provider;
using Kotas.Utils.Data.UnitOfWork.Provider;
using Kotas.Utils.Data.UnitOfWork.Registry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Kotas.Utils.Data.Native
{
    public static class Bootstrap
    {
        private static string _defaultConnectionString = "default";

        public static void SetDefaultConnectionString(string connectionString)
        {
            Validations.ValidateInput(connectionString, nameof(connectionString));

            _defaultConnectionString = connectionString;
        }

        public static void AddNativeDataAccess(this IServiceCollection services, IConfigurationRoot configuration)
        {
            Validations.ValidateInput(services, nameof(IServiceCollection));
            Validations.ValidateInput(configuration, nameof(IConfigurationRoot));

            var registry = new AsyncLocalUnitOfWorkRegistry();
            var uowProvider = new NativeUnitOfWorkProvider(registry, 
                (conStr, dbType) => CreateConnection(conStr, dbType, configuration));
            services.AddSingleton<IUnitOfWorkProvider>(uowProvider);

            var connectionProvider = new NativeConnectionProvider(
                (conStr, dbType) => CreateConnection(conStr, dbType, configuration));
            services.AddSingleton<INativeConnectionProvider>(connectionProvider);
        }

        private static IDbConnection CreateConnection(string connectionString, DatabaseType database, 
            IConfigurationRoot configuration)
        {
            connectionString = string.IsNullOrWhiteSpace(connectionString) ? _defaultConnectionString : connectionString;
            var isJustName = !connectionString.Contains(" ");

            if (isJustName)
            {
                connectionString = configuration.GetConnectionString(connectionString);
            }

            switch (database)
            {
                case DatabaseType.PostgreSql:
                    return new NpgsqlConnection(connectionString);
                default:
                    return new SqlConnection(connectionString);
            }
        }
    }
}
