using System;
using System.Data;
using Kotas.Utils.Common;
using Kotas.Utils.Data.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;
using Kotas.Utils.Data.UnitOfWork.Registry;

namespace Kotas.Utils.Data.Native.UnitOfWork.Provider
{
    internal class NativeUnitOfWorkProvider : UnitOfWorkProviderBase
    {
        private readonly  Func<string, DatabaseType, IDbConnection> _connectionFactory;

        public NativeUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<string, DatabaseType, IDbConnection> connectionFactory) : base(registry)
        {
            Validations.ValidateInput(connectionFactory, nameof(connectionFactory));

            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected sealed override IUnitOfWork CreateUnitOfWork(object parameter, DatabaseType databaseType, string connString)
        {
            var option = (parameter as DbConnectionOption?) ?? DbConnectionOption.ReuseParentConnection;
            return CreateUnitOfWork(() => _connectionFactory(connString, databaseType), option);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected virtual NativeUnitOfWork CreateUnitOfWork(Func<IDbConnection> connectionFactory, DbConnectionOption options)
        {
            return new NativeUnitOfWork(this, connectionFactory, options);
        }
    }
}
