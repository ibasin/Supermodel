using System.Collections.Generic;
using System.Data.Entity;

namespace Supermodel.Mvc
{
    public static class InitializerManager
    {
        public static void InitPerConfig<ContextT>(IList<IDatabaseInitializer<ContextT>> dbInitializers) where ContextT : DbContext
        {
            Database.SetInitializer(dbInitializers[Config.UseInitializerIdx]);
        }
    }
}
