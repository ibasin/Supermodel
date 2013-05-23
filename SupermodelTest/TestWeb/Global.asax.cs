using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Supermodel;
using Supermodel.Mvc;
using TestDomain.Supermodel;

namespace TestWeb
{
    using App_Start;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Supermodel init
            SupermodelInitialization.Init();
            InitializerManager.InitPerConfig(new List<IDatabaseInitializer<TestDbContext>> { new TestCreateDatabaseIfNotExists(), new TestDropCreateDatabaseIfModelChanges(), new TestDropCreateDatabaseAlways() });
        }
    }
}