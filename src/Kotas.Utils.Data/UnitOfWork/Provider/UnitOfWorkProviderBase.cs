using System;
using Kotas.Utils.Common;
using Kotas.Utils.Data.UnitOfWork.Registry;

namespace Kotas.Utils.Data.UnitOfWork.Provider
{
    /// <summary>
    /// A base implementation of unit of work provider.
    /// </summary>
    public abstract class UnitOfWorkProviderBase : IUnitOfWorkProvider
    {
        private readonly IUnitOfWorkRegistry _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkProviderBase"/> class.
        /// </summary>
        protected UnitOfWorkProviderBase(IUnitOfWorkRegistry registry)
        {
            Validations.ValidateInput(registry, nameof(registry));

            _registry = registry;
        }

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        public virtual IUnitOfWork Create()
        {
            return CreateCore(null, DatabaseType.SqlServer, null);
        }

        public IUnitOfWork Create(DatabaseType databaseType, string connectionString = null)
        {
            return CreateCore(null, databaseType, connectionString);
        }

        /// <summary>
        /// Creates a new unit of work instance with specified parameters.
        /// </summary>
        protected IUnitOfWork CreateCore(object parameter,  DatabaseType databaseType, string connectionString)
        {
            var uow = CreateUnitOfWork(parameter, databaseType, connectionString);
            _registry.RegisterUnitOfWork(uow);
            uow.Disposing += OnUnitOfWorkDisposing;
            return uow;
        }

        /// <summary>
        /// Creates the real unit of work instance.
        /// </summary>
        protected abstract IUnitOfWork CreateUnitOfWork(object parameter, DatabaseType databaseType, string connectionString);


        /// <summary>
        /// Called when the unit of work is being disposed.
        /// </summary>
        private void OnUnitOfWorkDisposing(object sender, EventArgs e)
        {
            _registry.UnregisterUnitOfWork((IUnitOfWork)sender);
        }


        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        public IUnitOfWork GetCurrent()
        {
            return _registry.GetCurrent();
        }
    }
}
