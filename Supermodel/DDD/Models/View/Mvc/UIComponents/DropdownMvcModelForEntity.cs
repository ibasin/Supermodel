using System;
using System.Globalization;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Repository;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class DropdownMvcModelForEntity : DropdownMvcModel, ICustomMapper
    {
        public virtual object MapFromObjectCustom(object obj, Type objType)
        {
            if (!objType.IsEntityType()) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            var entity = (IEntity)obj;
            SelectedValue = (entity == null) ? "" : entity.Id.ToString(CultureInfo.InvariantCulture);

            return this;
        }
        public virtual object MapToObjectCustom(object obj, Type objType)
        {
            if (!objType.IsEntityType()) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            if (string.IsNullOrEmpty(SelectedValue)) return null;

            var id = int.Parse(SelectedValue);
            var entity = (IEntity)obj;
            if (entity != null && entity.Id == id) return entity;

            var repo = RepoFactory.CreateForRuntimeType(objType);
            var newEntity = (IEntity)repo.ExecuteMethod("GetById", id);
            return newEntity;
        }
    }
}
