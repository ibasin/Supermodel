using System.Web.Mvc;

namespace Supermodel.Mvc.Controllers
{
    public interface ICRUDController
    {
        ActionResult List();

        [HttpDelete]
        ActionResult Edit(int id, HttpDelete ignore);

        [HttpGet]
        ActionResult Edit(int id, HttpGet ignore);

        [HttpPut]
        ActionResult Edit(int id, HttpPut ignore);

        [HttpPost]
        ActionResult Edit(int id, HttpPost ignore);

        [HttpGet]
        ActionResult GetBinaryFile(int id, string pn, HttpGet ignore);

        [HttpGet]
        ActionResult DeleteBinaryFile(int id, string pn, HttpDelete ignore);
    }
}