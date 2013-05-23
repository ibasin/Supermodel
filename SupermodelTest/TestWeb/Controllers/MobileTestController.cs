using System.Web.Mvc;
using Supermodel.DDD.UnitOfWork;
using Supermodel.Mvc;
using Supermodel.Mvc.Extensions;
using TestDomain.Entities;
using TestDomain.Supermodel;

namespace TestWeb.Controllers
{
    public class MobileTestPeopleController : TestCRUDController<TestPerson, TestPersonMobileMvcModel>
    {
        protected override ActionResult AfterCreate(int id, TestPerson entityItem, TestPersonMobileMvcModel mvcModelItem)
        {
            DbContextAmbientContext<TestDbContext>.CurrentDbContext.SaveChanges();
            TempData.Supermodel().NextPageModalMessage = "Person created successfully!";
            return this.Supermodel().RedirectToActionStrong(x => x.Edit(entityItem.Id, new HttpGet()));
        }
    }
    public class MobileTestChildrenController : TestChildCRUDController<TestChild, TestPerson, TestChildMobileMvcModel>
    {
        public override ActionResult List(int? parentId)
        {
            return this.Supermodel().RedirectToActionStrong<MobileTestPeopleController>(x => x.Edit((int)parentId, new HttpGet())); 
        }
    }
}
