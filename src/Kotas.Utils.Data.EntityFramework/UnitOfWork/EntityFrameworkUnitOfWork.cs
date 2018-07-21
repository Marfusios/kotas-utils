using System;
using System.Threading;
using System.Threading.Tasks;
using Kotas.Utils.Data.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;
using Microsoft.EntityFrameworkCore;

namespace Kotas.Utils.Data.EntityFramework.UnitOfWork
{
    public class EntityFrameworkUnitOfWork : UnitOfWorkBase
    {
        private readonly bool _hasOwnContext;

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        public DbContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWork"/> class.
        /// </summary>
        public EntityFrameworkUnitOfWork(IUnitOfWorkProvider provider, Func<DbContext> dbContextFactory, DbConnectionOption options)
        {
            if (options == DbConnectionOption.ReuseParentConnection)
            {
                if (provider.GetCurrent() is EntityFrameworkUnitOfWork parentUow)
                {
                    Context = parentUow.Context;
                    return;
                }
            }

            Context = dbContextFactory();
            _hasOwnContext = true;
        }

        /// <summary>
        /// Commits this instance when we have to.
        /// </summary>
        public override Task CommitAsync()
        {
            if (_hasOwnContext)
            {
                return base.CommitAsync();
            }
            return Task.FromResult(true);
        }

        protected override async Task CommitAsyncCore(CancellationToken cancellationToken)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        protected override void DisposeCore()
        {
            if (_hasOwnContext)
            {
                Context?.Dispose();
            }
        }

        public static DbContext TryGetDbContext(IUnitOfWorkProvider provider)
        {
            if (!(provider.GetCurrent() is EntityFrameworkUnitOfWork uow))
            {
                throw new InvalidOperationException("Must be used inside of unit of work. Did you forget call UowProvider.Create() inside 'using' block?");
            }
            return uow.Context;
        }
    }
}
