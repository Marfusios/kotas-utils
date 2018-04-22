using System.Collections.Generic;
using System.Threading;

namespace Kotas.Utils.Data.UnitOfWork.Registry
{
    /// <summary>
    /// A unit of work storage which persists the unit of work instances in a AsyncLocal object.
    /// </summary>
    public class AsyncLocalUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {

        private readonly AsyncLocal<Stack<IUnitOfWork>> _stack = new AsyncLocal<Stack<IUnitOfWork>>();

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected override Stack<IUnitOfWork> GetStack()
        {
            if (_stack.Value == null)
            {
                _stack.Value = new Stack<IUnitOfWork>();
            }
            return _stack.Value;
        }
    }
}
