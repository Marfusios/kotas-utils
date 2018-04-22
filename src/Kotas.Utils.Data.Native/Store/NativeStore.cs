using System.Data;
using Kotas.Utils.Common;
using Kotas.Utils.Data.Native.UnitOfWork;
using Kotas.Utils.Data.UnitOfWork.Provider;

namespace Kotas.Utils.Data.Native.Store
{
    public abstract class NativeStore : IStore
    {
        protected NativeStore(IUnitOfWorkProvider provider)
        {
            Validations.ValidateInput(provider, nameof(provider));

            UowProvider = provider;
        }

        protected IUnitOfWorkProvider UowProvider { get; }
        protected IDbTransaction Transaction => NativeUnitOfWork.TryGetDbTransaction(UowProvider);
        protected IDbConnection Connection => Transaction.Connection;
    }
}
