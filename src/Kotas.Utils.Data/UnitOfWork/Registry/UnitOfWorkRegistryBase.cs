using System;
using System.Collections.Generic;
using System.Linq;

namespace Kotas.Utils.Data.UnitOfWork.Registry
{
    /// <summary>
    /// A base implementation of unit of work registry.
    /// </summary>
    public abstract class UnitOfWorkRegistryBase : IUnitOfWorkRegistry
    {
        private readonly IUnitOfWorkRegistry _alternateRegistry;

        /// <summary>
        /// Gets the alternate registry and throws an exception when there is no such registry configured.
        /// </summary>
        protected IUnitOfWorkRegistry AlternateRegistry
        {
            get
            {
                if (_alternateRegistry == null)
                {
                    throw new InvalidOperationException($"The {GetType()} was not able to provide current unit of work and there is no alternate registry configured!");
                }
                return _alternateRegistry;
            }
        }

        public UnitOfWorkRegistryBase(IUnitOfWorkRegistry alternateRegistry = null)
        {
            _alternateRegistry = alternateRegistry;
        }


        /// <summary>
        /// Gets the stack of currently active unit of work objects.
        /// If the registry is unable to provide such stack, it should return null to let the caller to use alternate registry.
        /// </summary>
        protected abstract Stack<IUnitOfWork> GetStack();


        /// <summary>
        /// Registers a new unit of work.
        /// </summary>
        public void RegisterUnitOfWork(IUnitOfWork unitOfWork)
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack == null)
            {
                AlternateRegistry.RegisterUnitOfWork(unitOfWork);
            }
            else
            {
                unitOfWorkStack.Push(unitOfWork);
            }
        }

        /// <summary>
        /// Unregisters a specified unit of work.
        /// </summary>
        public void UnregisterUnitOfWork(IUnitOfWork unitOfWork)
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack == null)
            {
                AlternateRegistry.UnregisterUnitOfWork(unitOfWork);
            }
            else
            {
                if (unitOfWorkStack.Any())
                {
                    if (unitOfWorkStack.Pop() == unitOfWork)
                    {
                        return;
                    }
                }
                throw new InvalidOperationException("Some of the unit of works was not disposed correctly!");
            }
        }

        /// <summary>
        /// Gets the unit of work in the current scope.
        /// </summary>
        public IUnitOfWork GetCurrent()
        {
            var unitOfWorkStack = GetStack();
            if (unitOfWorkStack == null)
            {
                return AlternateRegistry.GetCurrent();
            }

            if (unitOfWorkStack.Count == 0)
            {
                return null;
            }
            else
            {
                return unitOfWorkStack.Peek();
            }
        }
    }
}
