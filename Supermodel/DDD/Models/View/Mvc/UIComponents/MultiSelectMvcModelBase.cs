using System;
using System.Collections.Generic;
using System.Globalization;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Repository;
using ReflectionMapper;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public abstract class MultiSelectMvcModelBase<MvcModelT> : MultiSelectMvcModelCore, ICustomMapper where MvcModelT : MvcModelForEntityCore
    {
        public object MapFromObjectCustom(object obj, Type objType)
        {
            var iCollectionInnerType = objType.GetICollectionGenericArg();
            if (!iCollectionInnerType.IsEntityType()) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            Options = ((ISqlDataRepoGenericTypeIgnorant)RepoFactory.CreateForRuntimeType(iCollectionInnerType)).GetMultiSelectOptions<MvcModelT>();
            if (obj == null) return this;
            foreach (var entity in (IEnumerable<IEntity>)obj)
            {
                // ReSharper disable AccessToModifiedClosure
                var match = Options.Find(x => x.Value == entity.Id.ToString(CultureInfo.InvariantCulture));
                // ReSharper restore AccessToModifiedClosure
                if (match != null) match.Selected = true;
            }

            return this;
        }
        public object MapToObjectCustom(object obj, Type objType)
        {
            var iCollectionInnerType = objType.GetICollectionGenericArg();
            if (!iCollectionInnerType.IsEntityType()) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            object collection;
            //if (objType.IsGenericType) collection = obj ?? ReflectionHelper.CreateGenericType(objType.GetGenericTypeDefinition(), objType.GetGenericArguments());
            if (objType.IsGenericType) collection = obj ?? ReflectionHelper.CreateGenericType(typeof(List<>), objType.GetGenericArguments()[0]);
            else collection = obj ?? ReflectionHelper.CreateType(objType);
            var repo = RepoFactory.CreateForRuntimeType(iCollectionInnerType);

            //first clear the collection
            collection.ExecuteMethod("Clear");

            //Then add the new ones
            foreach (var option in Options)
            {
                if (!option.Selected) continue;
                var id = int.Parse(option.Value);

                var newEntity = (IEntity)repo.ExecuteMethod("GetById", id);
                collection.ExecuteMethod("Add", newEntity);
            }
            return collection;
        }
    }
}
