using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kotas.Utils.Data.UnitOfWork
{
    /// <summary>
    /// An interface that represents a boundary of a business transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Asynchronously commits the changes made inside this unit of work.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Asynchronously commits the changes made inside this unit of work.
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Registers an action to be applied after the work is committed.
        /// </summary>
        void RegisterAfterCommitAction(Action action);

        /// <summary>
        /// Occurs when this unit of work is disposed.
        /// </summary>
        event EventHandler Disposing;

    }
}
