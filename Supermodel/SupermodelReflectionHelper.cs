using System;
using System.Collections.Generic;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.WebApi;

namespace Supermodel
{
    public static class SupermodelReflectionHelper
    {
        public static bool IsEntityType(this Type type)
        {
            return type != null && typeof (IEntity).IsAssignableFrom(type);
        }

        public static bool IsApiModelType(this Type type)
        {
            return type != null && ReflectionHelper.IsClassADerivedFromClassB(type, typeof(ApiModelForEntity<>));
        }

        //public static bool IsCollectionOfEntities(Type type)
        //{
        //    var icollectionInterfaceType = type.GetInterface("Icollection`1");
        //    if (icollectionInterfaceType == null) return false;
        //    var entityType = icollectionInterfaceType.GetGenericArguments()[0];
        //    return IsEntityType(entityType);
        //}
        public static Type GetICollectionGenericArg(this Type type)
        {
            //If the type is ICollection
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)) return type.GetGenericArguments()[0];

            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICollection<>)) return @interface.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
