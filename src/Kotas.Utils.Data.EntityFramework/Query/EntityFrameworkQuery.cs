using System;
using Kotas.Utils.Common;
using Kotas.Utils.Data.EntityFramework.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;
using Microsoft.EntityFrameworkCore;

namespace Kotas.Utils.Data.EntityFramework.Query
{
    public abstract class EntityFrameworkQuery : IQuery
    {
        protected EntityFrameworkQuery(IUnitOfWorkProvider provider)
        {
            Validations.ValidateInput(provider, nameof(provider));

            UowProvider = provider;
        }

        protected IUnitOfWorkProvider UowProvider { get; }

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        protected DbContext ContextBase => EntityFrameworkUnitOfWork.TryGetDbContext(UowProvider);

        /// <summary>
        /// Gets the DbContext auto casted to type of <see cref="TContext"/>.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        protected TContext Context<TContext>() where TContext: DbContext
        {
            if(!(ContextBase is TContext casted))
                throw new InvalidOperationException($"Current DbContext is not of type {typeof(TContext)}");
            return casted;
        }
    }
}
