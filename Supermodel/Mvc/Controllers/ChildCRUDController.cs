using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.UIComponents;
using Supermodel.DDD.Repository;
using Supermodel.DDD.UnitOfWork;
using ReflectionMapper;
using Supermodel.Mvc.Extensions;

namespace Supermodel.Mvc.Controllers
{
    public abstract class ChildCRUDController<EntityT, ParentEntityT, ChildMvcModelT, DbContextT> : Controller, IChildCRUDController
        where EntityT : class, IEntity, new()
        where ParentEntityT : class, IEntity, new()
        where ChildMvcModelT : ChildMvcModelForEntity<EntityT, ParentEntityT>, new()
        where DbContextT : EFDbContext, new()
    {
        public abstract ActionResult List(int? parentId);

        [HttpDelete]
        public virtual ActionResult Edit(int id, int? parentId, HttpDelete ignore)
        {
            EntityT entityItem = null;
            using (new EFUnitOfWork<DbContextT>())
            {
                try
                {
                    entityItem = RepoFactory.Create<EntityT>().GetById(id);
                    if (parentId == null)
                    {
                        var mvcModelItem = (ChildMvcModelT)new ChildMvcModelT().MapFromObject(entityItem);
                        var parent = mvcModelItem.GetParentEntity(entityItem);
                        if (parent != null) parentId = parent.Id;
                    }
                    entityItem.Delete();
                }
                catch (UnableToDeleteException ex)
                {
                    DbContextAmbientContext<DbContextT>.CurrentDbContext.CommitOnDispose = false; //rollback the transaction
                    TempData.Supermodel().NextPageModalMessage = ex.Message;
                }
                catch (Exception)
                {
                    DbContextAmbientContext<DbContextT>.CurrentDbContext.CommitOnDispose = false; //rollback the transaction
                    TempData.Supermodel().NextPageModalMessage = "PROBLEM!!!\\n\\nUnable to delete. Most likely reason: references from other entities.";
                }
            }
            return AfterDelete(id, parentId, entityItem);
        }

        [HttpGet]
        public virtual ActionResult Edit(int id, int? parentId, HttpGet ignore)
        {
            using (new EFUnitOfWork<DbContextT>(ReadOnly.Yes))
            {
                ChildMvcModelT mvcModelItem;
                if (id == 0)
                {
                    mvcModelItem = new ChildMvcModelT { ParentId = parentId }; //We set parentID twice, in case we may need it during MapFromObject
                    mvcModelItem = (ChildMvcModelT)mvcModelItem.MapFromObject(new EntityT().ConstructVirtualProperties());
                    mvcModelItem.ParentId = parentId;
                    return View(mvcModelItem);
                }

                var entityItem = RepoFactory.Create<EntityT>().GetById(id);
                mvcModelItem = (ChildMvcModelT)new ChildMvcModelT().MapFromObject(entityItem);
                return View(mvcModelItem);
            }
        }

        [HttpPut]
        public virtual ActionResult Edit(int id, int? parentId, HttpPut ignore)
        {
            using (new EFUnitOfWork<DbContextT>())
            {
                if (id == 0) throw new SupermodelException("CRUDControllerBase.Edit[Post]: id == 0");

                var entityItem = RepoFactory.Create<EntityT>().GetById(id);
                ChildMvcModelT mvcModelItem;
                try
                {
                    entityItem = TryUpdateEntity(entityItem, null, out mvcModelItem);
                    if (parentId == null)
                    {
                        var parent = mvcModelItem.GetParentEntity(entityItem);
                        if (parent != null) parentId = parent.Id;
                    }
                }
                catch (ModelStateInvalidException ex)
                {
                    DbContextAmbientContext<DbContextT>.CurrentDbContext.CommitOnDispose = false; //rollback the transaction
                    return View(ex.Model);
                }
                return AfterUpdate(id, parentId, entityItem, mvcModelItem);
            }
        }

        [HttpPost]
        public virtual ActionResult Edit(int id, int? parentId, HttpPost ignore)
        {
            using (new EFUnitOfWork<DbContextT>())
            {
                if (id != 0) throw new SupermodelException("CRUDControllerBase.Edit[Put]: id != 0");

                EntityT entityItem = (EntityT)new EntityT().ConstructVirtualProperties();
                ChildMvcModelT mvcModelItem;
                try
                {
                    entityItem = TryUpdateEntity(entityItem, parentId, out mvcModelItem);
                }
                catch (ModelStateInvalidException ex)
                {
                    DbContextAmbientContext<DbContextT>.CurrentDbContext.CommitOnDispose = false; //rollback the transaction
                    return View(ex.Model);
                }
                entityItem.Add();
                return AfterCreate(id, parentId, entityItem, mvcModelItem);
            }
        }

        [HttpGet]
        public virtual ActionResult GetBinaryFile(int id, int? parentId, string pn, HttpGet ignore)
        {
            using (new EFUnitOfWork<DbContextT>(ReadOnly.Yes))
            {
                var mvcModelItem = (ChildMvcModelT)new ChildMvcModelT().MapFromObject(RepoFactory.Create<EntityT>().GetById(id));
                var file = (BinaryFileMvcModel)mvcModelItem.PropertyGet(pn);
                const string mimeType = "application/octet-stream";
                //var mimeType = Common.GetContentTypeHeader(file.Name);
                if (file == null || file.IsEmpty) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                return File(file.BinaryContent, mimeType, file.Name);
            }
        }

        [HttpGet]
        public virtual ActionResult DeleteBinaryFile(int id, int? parentId, string pn, HttpDelete ignore)
        {
            using (new EFUnitOfWork<DbContextT>())
            {
                var entityItem = RepoFactory.Create<EntityT>().GetById(id);
                var mvcModelItem = (ChildMvcModelT)new ChildMvcModelT().MapFromObject(entityItem);
                if (parentId == null)
                {
                    var parent = mvcModelItem.GetParentEntity(entityItem);
                    if (parent != null) parentId = parent.Id;
                }

                //see if pn is a required property
                if (Attribute.GetCustomAttribute(typeof(ChildMvcModelT).GetProperty(pn), typeof(RequiredAttribute), true) != null)
                {
                    TempData.Supermodel().NextPageModalMessage = "Cannot delete required field";
                    var routeValues = new RouteValueDictionary(ControllerContext.RouteData.Values);
                    routeValues["Action"] = "Edit";
                    routeValues.Add("parentId", parentId);
                    return RedirectToRoute(routeValues);
                }

                var file = (BinaryFileMvcModel)mvcModelItem.PropertyGet(pn);

                file.Empty();
                entityItem = (EntityT)mvcModelItem.MapToObject(entityItem);
                return AfterBinaryDelete(id, parentId, entityItem, mvcModelItem);
            }
        }

        #region Protected Methods & Properties

        protected virtual ActionResult AfterUpdate(int id, int? parentId, EntityT entityItem, ChildMvcModelT mvcModelItem)
        {
            return RedirectToAction("List", new { parentId });
        }

        protected virtual ActionResult AfterCreate(int id, int? parentId, EntityT entityItem, ChildMvcModelT mvcModelItem)
        {
            // ReSharper disable Mvc.ActionNotResolved
            return RedirectToAction("List", new { parentId });
            // ReSharper restore Mvc.ActionNotResolved
        }
        //protected virtual ActionResult AfterCreateFinally(int id, int? parentId, EntityT entityItem, ChildMvcModelT mvcModelItem)
        //{
        //    // ReSharper disable Mvc.ActionNotResolved
        //    return RedirectToAction("List", new { parentId });
        //    // ReSharper restore Mvc.ActionNotResolved
        //}

        protected virtual ActionResult AfterDelete(int id, int? parentId, EntityT entityItem)
        {
            return RedirectToAction("List", new { parentId });
        }

        protected virtual ActionResult AfterBinaryDelete(int id, int? parentId, EntityT entityItem, ChildMvcModelT mvcModelItem)
        {
            return RedirectToAction("Edit", new { id, parentId });
        }
        #endregion 

        #region Protected helper methods
        //this methods will catch validation exceptions that happen during mapping from mvc to domain (when it runs validation for mvc model by creating a domain object)
        protected virtual EntityT TryUpdateEntity(EntityT entityItem, int? parentId, out ChildMvcModelT mvcModelItem)
        {
            mvcModelItem = (ChildMvcModelT)new ChildMvcModelT().MapFromObject(entityItem);
            if (parentId != null) mvcModelItem.ParentId = parentId;
            try
            {
                TryUpdateModel(mvcModelItem);
                entityItem = (EntityT)mvcModelItem.MapToObject(entityItem);
                if (ModelState.IsValid != true) throw new ModelStateInvalidException(mvcModelItem);
                return entityItem;
            }
            catch (ValidationResultException ex)
            {
                foreach (var validationResult in ex.ValidationResultList)
                {
                    foreach (var memeberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memeberName, validationResult.ErrorMessage);
                    }
                }
                throw new ModelStateInvalidException(mvcModelItem);
            }
        }
        #endregion
    }
}
