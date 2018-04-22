using System;
using Kotas.Utils.Data.Native.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;
using Kotas.Utils.Data.UnitOfWork.Registry;
using Microsoft.EntityFrameworkCore;

namespace Kotas.Utils.Data.EntityFramework.UnitOfWork
{
    public class EntityFrameworkUnitOfWorkProvider : UnitOfWorkProviderBase
    {
        private readonly Func<string, DatabaseType, DbContext> _dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWorkProvider"/> class.
        /// </summary>
        public EntityFrameworkUnitOfWorkProvider(IUnitOfWorkRegistry registry, Func<string, DatabaseType, DbContext> dbContextFactory) : base(registry)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected sealed override IUnitOfWork CreateUnitOfWork(object parameter, DatabaseType databaseType, string connString)
        {
            var option = (parameter as DbConnectionOption?) ?? DbConnectionOption.ReuseParentConnection;
            return CreateUnitOfWork(() => _dbContextFactory(connString, databaseType), option);
        }

        /// <summary>
        /// Creates the unit of work.
        /// </summary>
        protected virtual EntityFrameworkUnitOfWork CreateUnitOfWork(Func<DbContext> contextFactory, DbConnectionOption options)
        {
            return new EntityFrameworkUnitOfWork(this, contextFactory, options);
        }
    }
}
