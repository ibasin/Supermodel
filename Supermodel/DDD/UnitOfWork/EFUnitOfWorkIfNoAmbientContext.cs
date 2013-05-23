using System;

namespace Supermodel.DDD.UnitOfWork
{
    public class EFUnitOfWorkIfNoAmbientContext<DbContextT> : IDisposable where DbContextT : EFDbContext, new()
    {
        public DbContextT DbContext { get; private set; }

        public EFUnitOfWorkIfNoAmbientContext(MustBeWritable mustBeWritable)
        {
            bool? createNewContext;
            var mustBeWritableBool = (mustBeWritable == MustBeWritable.Yes);
            if (DbContextAmbientContext<DbContextT>.HasDbContext())
            {
                var ambientContextReadOnlyBool = DbContextAmbientContext<DbContextT>.CurrentDbContext.IsReadOnly;
                if (mustBeWritableBool && ambientContextReadOnlyBool) createNewContext = true;
                else createNewContext = false;
            }
            else
            {
                createNewContext = true;
            }

            if ((bool)createNewContext)
            {
                DbContext = new DbContextT();
                if (!mustBeWritableBool) DbContext.MakeReadOnly();
                DbContextAmbientContext<DbContextT>.PushDbContext(this.DbContext);
            }
            else
            {
                DbContext = null;
            }
        }

        public void Dispose()
        {
            if (DbContext == null) return;

            DbContext.Dispose();
            var dbContext = DbContextAmbientContext<DbContextT>.PopDbContext();
            // ReSharper disable PossibleUnintendedReferenceComparison
            if (dbContext != DbContext) throw new SupermodelException(string.Format("EFUnitOfWork: POP on Dispose popped mismatched DB Context."));
            // ReSharper restore PossibleUnintendedReferenceComparison
        }
    }
}
