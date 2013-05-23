using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.Mvc.Controllers;

namespace TestDomain.Supermodel
{
    public abstract class TestCRUDController<EntityT, MvcModelT> : CRUDController<EntityT, MvcModelT, TestDbContext>
        where EntityT : class, IEntity, new()
        where MvcModelT : MvcModelForEntity<EntityT>, new()
    {}
}
