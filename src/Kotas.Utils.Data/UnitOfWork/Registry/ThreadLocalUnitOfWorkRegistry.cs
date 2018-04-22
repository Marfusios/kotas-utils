using System.Collections.Generic;
using System.Threading;

namespace Kotas.Utils.Data.UnitOfWork.Registry
{
    /// <summary>
    /// A unit of work storage which persists the unit of work instances in a ThreadLocal object.
    /// </summary>
    public class ThreadLocalUnitOfWorkRegistry : UnitOfWorkRegistryBase
    {

        private readonly ThreadLocal<Stack<IUnitOfWork>> _stack
            = new ThreadLocal<Stack<IUnitOfWork>>(() => new Stack<IUnitOfWork>());

        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// </summary>
        protected override Stack<IUnitOfWork> GetStack()
        {
            return _stack.Value;
        }
    }
}
