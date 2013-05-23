using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public abstract class CRUDController<EntityT, MvcModelT, DbContextT> : Controller, ICRUDController
        where EntityT : class, IEntity, new()
        where MvcModelT : MvcModelForEntity<EntityT>, new()
        where DbContextT : EFDbContext, new()
    {
        public virtual ActionResult List()
        {
            using (new EFUnitOfWork<DbContextT>(ReadOnly.Yes))
            {
                var entities = RepoFactory.Create<EntityT>().GetAll();
                var mvcModels = new List<MvcModelT>();
                mvcModels = (List<MvcModelT>)mvcModels.MapFromObject(entities);
                mvcModels = mvcModels.OrderBy(p => p.Label).ToList();
                return View(mvcModels);
            }
        }

        [HttpDelete]
        public virtual ActionResult Edit(int id, HttpDelete ignore)
        {
            EntityT entityItem = null;
            using (new EFUnitOfWork<DbContextT>())
            {
                try
                {
                    entityItem = RepoFactory.Create<EntityT>().GetById(id);
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
            return AfterDelete(id, entityItem);
        }

        [HttpGet]
        public virtual ActionResult Edit(int id, HttpGet ignore)
        {
            using (new EFUnitOfWork<DbContextT>(ReadOnly.Yes))
            {
                if (id == 0) return View(new MvcModelT().MapFromObject(new EntityT().ConstructVirtualProperties()));

                var entityItem = RepoFactory.Create<EntityT>().GetById(id);
                var mvcModelItem = (MvcModelT)new MvcModelT().MapFromObject(entityItem);

                return View(mvcModelItem);
            }
        }

        [HttpPut]
        public virtual ActionResult Edit(int id, HttpPut ignore)
        {
            using (new EFUnitOfWork<DbContextT>())
            {
                if (id == 0) throw new SupermodelException("CRUDControllerBase.Edit[Put]: id == 0");

                var entityItem = RepoFactory.Create<EntityT>().GetById(id);
                MvcModelT mvcModelItem;
                try
                {
                    entityItem = TryUpdateEntity(entityItem, out mvcModelItem);
                }
                catch (ModelStateInvalidException ex)
                {
                    DbContextAmbientContext<DbContextT>.CurrentDbContext.CommitOnDispose = false; //rollback the transaction
                    return View(ex.Model);
                }
                return AfterUpdate(id, entityItem, mvcModelItem);
            }
        }

        [HttpPost]
        public virtual ActionResult Edit(int id, HttpPost ignore)
        {
            using (new EFUnitOfWork<DbContextT>())
            {
                if (id != 0) throw new SupermodelException("CRUDControllerBase.Edit[Post]: id != 0");

                var entityItem = (EntityT)new EntityT().ConstructVirtualProperties();
                MvcModelT mvcModelItem;
                try
                {
                    entityItem = TryUpdateEntity(entityItem, out mvcModelItem);
                }
                catch (ModelStateInvalidException ex)
                {
                    DbContextAmbientContext<DbContextT>.CurrentDbContext.CommitOnDispose = false; //rollback the transaction
                    return View(ex.Model);
                }
                entityItem.Add();
                
                return AfterCreate(id, entityItem, mvcModelItem);
            }
        }

        [HttpGet]
        public virtual ActionResult GetBinaryFile(int id, string pn, HttpGet ignore)
        {
            using (new EFUnitOfWork<DbContextT>(ReadOnly.Yes))
            {
                var mvcModelItem = (MvcModelT)new MvcModelT().MapFromObject(RepoFactory.Create<EntityT>().GetById(id));
                var file = (BinaryFileMvcModel) mvcModelItem.PropertyGet(pn);
                const string mimeType = "application/octet-stream";
                //var mimeType = Common.GetContentTypeHeader(file.Name);
                if (file == null || file.IsEmpty) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                return File(file.BinaryContent, mimeType, file.Name);
            }
        }

        [HttpGet]
        public virtual ActionResult DeleteBinaryFile(int id, string pn, HttpDelete ignore)
        {
            using (new EFUnitOfWork<DbContextT>())
            {
                var entityItem = RepoFactory.Create<EntityT>().GetById(id);
                var mvcModelItem = (MvcModelT)new MvcModelT().MapFromObject(entityItem);

                //see if pn is a required property
                if (Attribute.GetCustomAttribute(typeof(MvcModelT).GetProperty(pn), typeof(RequiredAttribute), true) != null)
                {
                    TempData.Supermodel().NextPageModalMessage = "Cannot delete required field";
                    var routeValues = new RouteValueDictionary(ControllerContext.RouteData.Values);
                    routeValues["Action"] = "Edit";
                    return RedirectToRoute(routeValues);
                }

                var file = (BinaryFileMvcModel) mvcModelItem.PropertyGet(pn);

                file.Empty();
                entityItem = (EntityT)mvcModelItem.MapToObject(entityItem);
                return AfterBinaryDelete(id, entityItem, mvcModelItem);
            }
        }

        #region Protected Methods & Properties
        protected virtual ActionResult AfterUpdate(int id, EntityT entityItem, MvcModelT mvcModelItem)
        {
            // ReSharper disable Mvc.ActionNotResolved
            return RedirectToAction("List");
            // ReSharper restore Mvc.ActionNotResolved
        }
        protected virtual ActionResult AfterCreate(int id, EntityT entityItem, MvcModelT mvcModelItem)
        {
            // ReSharper disable Mvc.ActionNotResolved
            return RedirectToAction("List");
            // ReSharper restore Mvc.ActionNotResolved
        }
        protected virtual ActionResult AfterDelete(int id, EntityT entityItem)
        {
            // ReSharper disable Mvc.ActionNotResolved
            return RedirectToAction("List");
            // ReSharper restore Mvc.ActionNotResolved
        }
        protected virtual ActionResult AfterBinaryDelete(int id, EntityT entityItem, MvcModelT mvcModelItem)
        {
            // ReSharper disable Mvc.ActionNotResolved
            return RedirectToAction("Edit", new {id});
            // ReSharper restore Mvc.ActionNotResolved
        }
        #endregion

        #region Protected helper methods
        //this methods will catch validation exceptions that happen during mapping from mvc to domain (when it runs validation for mvc model by creating a domain object)
        protected virtual EntityT TryUpdateEntity(EntityT entityItem, out MvcModelT mvcModelItem)
        {
            mvcModelItem = (MvcModelT)new MvcModelT().MapFromObject(entityItem);
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
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
                throw new ModelStateInvalidException(mvcModelItem);
            }
        }
        #endregion
    }
}