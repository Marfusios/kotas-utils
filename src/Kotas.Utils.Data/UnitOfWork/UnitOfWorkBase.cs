using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kotas.Utils.Data.UnitOfWork
{
    /// <summary>
    ///     A base implementation of unit of work object.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private readonly List<Action> _afterCommitActions = new List<Action>();
        private bool _isDisposed;

        public event EventHandler Disposing;


        public virtual async Task CommitAsync()
        {
            await CommitAsync(default(CancellationToken));
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await CommitAsyncCore(cancellationToken);

            RunAfterCommitActions();
        }

        /// <summary>
        ///     Registers an action to be executed after the work is committed.
        /// </summary>
        public void RegisterAfterCommitAction(Action action)
        {
            _afterCommitActions.Add(action);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                OnDisposing();
                DisposeCore();
            }
        }

        private void RunAfterCommitActions()
        {
            foreach (var action in _afterCommitActions)
                action();
            _afterCommitActions.Clear();
        }

        /// <summary>
        ///     Performs the real asynchronously commit work.
        /// </summary>
        /// <param name="cancellationToken"></param>
        protected abstract Task CommitAsyncCore(CancellationToken cancellationToken);

        /// <summary>
        ///     Performs the real dispose work.
        /// </summary>
        protected abstract void DisposeCore();

        /// <summary>
        ///     Called when the unit of work is being disposed.
        /// </summary>
        protected virtual void OnDisposing()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }
    }
}
