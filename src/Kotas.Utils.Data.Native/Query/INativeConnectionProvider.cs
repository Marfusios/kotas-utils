using System.Data;

namespace Kotas.Utils.Data.Native.Query
{
    public interface INativeConnectionProvider
    {
        IDbConnection Create();
        IDbConnection Create(DatabaseType databaseType, string connectionString = null);
    }
}
