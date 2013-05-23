using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.Mvc.Controllers;

namespace TestDomain.Supermodel
{
    public abstract class TestChildCRUDController<EntityT, ParentEntityT, MvcModelT> : ChildCRUDController<EntityT, ParentEntityT, MvcModelT, TestDbContext>
        where EntityT : class, IEntity, new()
        where ParentEntityT : class, IEntity, new()
        where MvcModelT : ChildMvcModelForEntity<EntityT, ParentEntityT>, new()
    { }
}
