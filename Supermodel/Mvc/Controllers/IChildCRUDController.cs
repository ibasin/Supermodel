using System.Web.Mvc;

namespace Supermodel.Mvc.Controllers
{
    public interface IChildCRUDController
    {
        ActionResult List(int? parentId);

        [HttpDelete]
        ActionResult Edit(int id, int? parentId, HttpDelete ignore);

        [HttpGet]
        ActionResult Edit(int id, int? parentId, HttpGet ignore);

        [HttpPut]
        ActionResult Edit(int id, int? parentId, HttpPut ignore);

        [HttpPost]
        ActionResult Edit(int id, int? parentId, HttpPost ignore);

        [HttpGet]
        ActionResult GetBinaryFile(int id, int? parentId, string pn, HttpGet ignore);

        [HttpGet]
        ActionResult DeleteBinaryFile(int id, int? parentId, string pn, HttpDelete ignore);
    }
}