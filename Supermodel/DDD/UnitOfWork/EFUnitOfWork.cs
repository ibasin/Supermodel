using System;

namespace Supermodel.DDD.UnitOfWork
{
    public class EFUnitOfWork<DbContextT> : IDisposable where DbContextT : EFDbContext, new()
    {
        public DbContextT DbContext { get; private set; }

        public EFUnitOfWork(ReadOnly readOnly = ReadOnly.No)
        {
            DbContext = new DbContextT();
            if (readOnly == ReadOnly.Yes) DbContext.MakeReadOnly();
            DbContextAmbientContext<DbContextT>.PushDbContext(DbContext);
        }

        public void Dispose()
        {
            DbContext.Dispose();

            var dbContext = DbContextAmbientContext<DbContextT>.PopDbContext();

            // ReSharper disable PossibleUnintendedReferenceComparison
            if (dbContext != DbContext) throw new SupermodelException(string.Format("EFUnitOfWork: POP on Dispose popped mismatched DB Context."));
            // ReSharper restore PossibleUnintendedReferenceComparison
        }
    }
}
