namespace Kotas.Utils.Data.EntityFramework.UnitOfWork
{
    public enum DbConnectionOption
    {
        /// <summary>
        /// If we are already in another unit of work, its DbConnection will be reused and the changes will be committed after the outer unit of work commits.
        /// </summary>
        ReuseParentConnection = 0,

        /// <summary>
        /// This unit of work is standalone, has its own DbConnection and doesn't depend on any other unit of work instances.
        /// </summary>
        AlwaysCreateOwnConnection = 1
    }
}
