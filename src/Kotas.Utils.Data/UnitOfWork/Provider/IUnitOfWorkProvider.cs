namespace Kotas.Utils.Data.UnitOfWork.Provider
{
    /// <summary>
    /// An interface for unit of work provider which is responsible for creating and managing unit of work objects.
    /// </summary>
    public interface IUnitOfWorkProvider
    {

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        IUnitOfWork Create();

        /// <summary>
        /// Creates a new unit of work.
        /// For specified database and connection string
        /// </summary>
        IUnitOfWork Create(DatabaseType databaseType, string connectionString = null);

        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        IUnitOfWork GetCurrent();

    }
}
