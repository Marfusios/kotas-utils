using Kotas.Utils.Common;

namespace Kotas.Utils.Data.Native.Query
{
    public class NativeQuery : IQuery
    {
        protected NativeQuery(INativeConnectionProvider provider)
        {
            Validations.ValidateInput(provider, nameof(provider));

            ConnectionProvider = provider;
        }

        protected INativeConnectionProvider ConnectionProvider { get; }
    }
}
