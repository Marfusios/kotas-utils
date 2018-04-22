using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Kotas.Utils.Data.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;

namespace Kotas.Utils.Data.Native.UnitOfWork
{
    /// <summary>
    /// An implementation of unit of work for native approach (System.Data).
    /// </summary>
    internal class NativeUnitOfWork : UnitOfWorkBase
    {
        private readonly bool _hasOwnConnection;

        public IDbTransaction Transaction;
        public IDbConnection Connection { get; }


        public NativeUnitOfWork(IUnitOfWorkProvider provider, Func<IDbConnection> connectionFactory, DbConnectionOption options)
        {
            if (options == DbConnectionOption.ReuseParentConnection)
            {
                if (provider.GetCurrent() is NativeUnitOfWork parentUow)
                {
                    Transaction = parentUow.Transaction;
                    return;
                }
            }

            Connection = connectionFactory();
            Connection.Open();
            Transaction = Connection.BeginTransaction();
            _hasOwnConnection = true;
        }


        /// <summary>
        /// Commits this instance when we have to.
        /// </summary>
        public override Task CommitAsync()
        {
            if (_hasOwnConnection)
            {
                return base.CommitAsync();
            }
            return Task.FromResult(true);
        }

        protected override Task CommitAsyncCore(CancellationToken cancellationToken)
        {
            try
            {
                Transaction.Commit();
            }
            catch
            {
                Transaction.Rollback();
                throw;
            }
            finally
            {
                Transaction.Dispose();
                Transaction = Connection.BeginTransaction();
            }
            return Task.FromResult(true);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        protected override void DisposeCore()
        {
            if (_hasOwnConnection)
            {
                Transaction?.Dispose();
                Connection?.Dispose();
            }
        }

        public static IDbTransaction TryGetDbTransaction(IUnitOfWorkProvider provider)
        {
            if (!(provider.GetCurrent() is NativeUnitOfWork uow))
            {
                throw new InvalidOperationException("Must be used inside of unit of work. Did you forget call UowProvider.Create() inside 'using' block?");
            }
            return uow.Transaction;
        }
    }
}
