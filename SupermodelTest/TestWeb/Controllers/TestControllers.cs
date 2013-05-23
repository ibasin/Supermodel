using System.Web.Mvc;
using TestDomain.Entities;
using TestDomain.Supermodel;

namespace TestWeb.Controllers
{
    public class TestGroupsController : TestCRUDController<TestGroup, TestGroupMvcModel> { }
    public class TestPeopleController : TestCRUDController<TestPerson, TestPersonMvcModel> { }
    public class TestChildrenController : TestChildCRUDController<TestChild, TestPerson, TestChildMvcModel>
    {
        public override ActionResult List(int? parentId) { return RedirectToAction("Edit", "TestPeople", new { id = parentId }); }
    }
}
