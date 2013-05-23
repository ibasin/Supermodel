using System;
using System.Collections.Generic;

namespace Supermodel.DDD.UnitOfWork
{
    public static class DbContextAmbientContext<T> where T : EFDbContext
    {
        #region Methods and Properties
        static public T PopDbContext()
        {
            return (T)DbContextAmbientContextCore.PopDbContext();
        }
        static public void PushDbContext(T uow)
        {
            DbContextAmbientContextCore.PushDbContext(uow);
        }
        public static uint StackCount
        {
            get { return DbContextAmbientContextCore.StackCount; }
        }
        static public T CurrentDbContext
        {
            get { return (T)DbContextAmbientContextCore.CurrentDbContext; }
        }
        public static bool HasDbContext()
        {
            return StackCount > 0;
        }
        #endregion
    }

    public static class DbContextAmbientContextCore
    {
        #region Methods and Properties
        static public EFDbContext PopDbContext()
        {
            return _dbContextStackThreadSafe.Pop();
        }

        static public void PushDbContext(EFDbContext uow)
        {
            _dbContextStackThreadSafe.Push(uow);
        }

        public static uint StackCount
        {
            get { return (uint)_dbContextStackThreadSafe.Count; }
        }

        public static EFDbContext CurrentDbContext
        {
            get
            {
                try
                {
                    return _dbContextStackThreadSafe.Peek();
                }
                catch (InvalidOperationException)
                {
                    throw new SupermodelException("Current UnitOfWork does not exist. All database access oprations must be wrapped in 'using(new UnitOfWork())'");
                }
            }
        }
        #endregion

        #region Private variables
        [ThreadStatic] 
        private static Stack<EFDbContext> _dbContextStack;
        // ReSharper disable InconsistentNaming
        private static Stack<EFDbContext> _dbContextStackThreadSafe
        // ReSharper restore InconsistentNaming
        {
            get
            {
                if (_dbContextStack == null) _dbContextStack = new Stack<EFDbContext>();
                return _dbContextStack;
            }
        }
        #endregion
    }
}
