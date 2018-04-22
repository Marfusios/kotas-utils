using System;
using System.Data;

namespace Kotas.Utils.Data.Native.Query
{
    internal class NativeConnectionProvider : INativeConnectionProvider
    {
        private readonly Func<string, DatabaseType, IDbConnection> _connectionFactory;

        public NativeConnectionProvider(Func<string, DatabaseType, IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection Create()
        {
            return _connectionFactory(null, DatabaseType.SqlServer);
        }

        public IDbConnection Create(DatabaseType databaseType, string connectionString = null)
        {
            return _connectionFactory(connectionString, databaseType);
        }
    }
}
