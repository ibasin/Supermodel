﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Repository;

namespace Supermodel.DDD.Models.View.Mvc
{
    [Bind(Exclude = "ParentId")]
    abstract public class ChildMvcModelForEntity<EntityT, ParentEntityT> : MvcModelForEntity<EntityT>
        where EntityT : class, IEntity, new()
        where ParentEntityT : class, IEntity, new()
    {
        [ScaffoldColumn(false), NotRMapped]
        public virtual int? ParentId { get; set; }

        public abstract ParentEntityT GetParentEntity(EntityT entity);
        public abstract void SetParentEntity(EntityT entity, ParentEntityT parent);
        
        public override object MapFromObjectCustom(object obj, Type objType)
        {
            var result = (ChildMvcModelForEntity<EntityT, ParentEntityT>)base.MapFromObjectCustom(obj, objType);
            if (obj != null)
            {
                var parentEntity = GetParentEntity((EntityT) obj); 
                if (parentEntity == null) ParentId = null;
                else ParentId = parentEntity.Id;
            }
            return result;
        }

        public override object MapToObjectCustom(object obj, Type objType)
        {
            var result = (EntityT) base.MapToObjectCustom(obj, objType);
            if (result == null) return null;

            var parentEntity = GetParentEntity((EntityT) obj);
            if (parentEntity == null && ParentId != null || 
                parentEntity != null && parentEntity.Id != ParentId)
            {
                if (ParentId == null) SetParentEntity((EntityT)obj, null);
                else SetParentEntity((EntityT) obj, RepoFactory.Create<ParentEntityT>().GetById((int)ParentId));
            }
            return result;
        }
    }
}
