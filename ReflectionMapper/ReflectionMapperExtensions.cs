using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace ReflectionMapper
{
    public static class ReflectionMapperExtensions
    {
        public static object MapFromObject(this object me, object obj)
        {
            //first check if me implements ICustomMapper, if it does we use that. If custom mapper does not exist, we go property by property by calling the base method
            if (me is ICustomMapper) me = (me as ICustomMapper).MapFromObjectCustom(obj, obj.GetType());
            else me = me.MapFromObjectCustomBase(obj);
            return me;
        }
        public static object MapToObject(this object me, object obj)
        {
            //first check if me implements ICustomMapper, if it does we use that. If custom mapper does not exist, we go property by property by calling the base method
            if (me is ICustomMapper) obj = (me as ICustomMapper).MapToObjectCustom(obj, obj.GetType());
            else obj = me.MapToObjectCustomBase(obj);
            return obj;
        }

        public static object MapFromObjectCustomBase(this object me, object obj)
        {
            //if obj == null, we set me to null too, the caller must assign the result of this method
            if (obj == null) return null;

            //if me is null, we need to create an object
            if (me == null) throw new ReflectionMapperException("MapFromObjectCustomBase(): me is null");

            //ICollection<ICustomeMapper>
            if (AreCompatibleCollections(me.GetType(), obj.GetType()))
            {
                var objIEnumerable = (IEnumerable)obj;
                var myIEnumerableGenericArg = me.GetType().GetInterface(typeof(IEnumerable<>).Name).GetGenericArguments()[0];
                foreach (var objItemObj in objIEnumerable)
                {
                    me.ExecuteMethod("Add", objItemObj != null ? ReflectionHelper.CreateType(myIEnumerableGenericArg).ExecuteMethod("MapFromObjectCustom", objItemObj, objItemObj.GetType()) : null);
                }
                return me;
            }
            
            foreach (var property in me.GetType().GetProperties())
            {
                //If property is marked NotReflectionMappedAttribute, don't worry about this one
                if (Attribute.GetCustomAttribute(property, typeof(NotRMappedAttribute), true) != null) continue;

                //Find matching properties
                var myProperty = property;
                var objPropertyMeta = GetMatchingDomainProperty(me, myProperty, obj);

                //ICustomMapper
                if (myProperty.PropertyType.GetInterface(typeof(ICustomMapper).Name) != null)
                {
                    try
                    {
                        //Get the property we are mapping to
                        var objPropertyObj = objPropertyMeta.Obj.PropertyGet(objPropertyMeta.PropertyInfo.Name);

                        var myPropertyObj = me.PropertyGet(myProperty.Name);
                        if (myPropertyObj == null)
                        {
                            //we create blank object for the property in question
                            myPropertyObj = !myProperty.PropertyType.IsGenericType ? ReflectionHelper.CreateType(myProperty.PropertyType) : ReflectionHelper.CreateGenericType(myProperty.PropertyType.GetGenericTypeDefinition(), myProperty.PropertyType.GetGenericArguments()[0]);
                        }

                        //then we ask that property to map itself to the matching property of the object
                        // ReSharper disable PossibleNullReferenceException
                        (myPropertyObj as ICustomMapper).MapFromObjectCustom(objPropertyObj, objPropertyMeta.PropertyInfo.PropertyType);
                        // ReSharper restore PossibleNullReferenceException

                        me.PropertySet(myProperty.Name, myPropertyObj);
                    }
                    catch (Exception ex)
                    {
                        throw new PropertyCantBeAutomappedException(string.Format("Property '{0}' of class '{1}' can't be automapped to type '{2}' property '{3}' because ICustomMapper implementation threw an exception: {4}.", myProperty.Name, me.GetType().Name, obj.GetType().Name, objPropertyMeta.PropertyInfo.Name, ex.Message));
                    }
                }

                //IColeection<ICustomMapper>
                else if (AreCompatibleCollections(myProperty.PropertyType, objPropertyMeta.PropertyInfo.PropertyType))
                {
                    var objIEnumerable = (IEnumerable)objPropertyMeta.Obj.PropertyGet(objPropertyMeta.PropertyInfo.Name);
                    var myIEnumerableGenericArg = myProperty.PropertyType.GetInterface(typeof(IEnumerable<>).Name).GetGenericArguments()[0];
                    if (objIEnumerable != null)
                    {
                        me.PropertySet(myProperty.Name, ReflectionHelper.CreateGenericType(objIEnumerable.GetType().GetGenericTypeDefinition(), myIEnumerableGenericArg));
                        foreach (var objItemObj in objIEnumerable)
                        {
                            me.PropertyGet(myProperty.Name).ExecuteMethod("Add", objItemObj != null ? ReflectionHelper.CreateType(myIEnumerableGenericArg).ExecuteMethod("MapFromObjectCustom", objItemObj, objItemObj.GetType()) : null);
                        }
                    }
                }

                //This is the default case of when the types match exactly
                else if (myProperty.PropertyType == objPropertyMeta.PropertyInfo.PropertyType)
                {
                    me.PropertySet(myProperty.Name, objPropertyMeta.Obj.PropertyGet(objPropertyMeta.PropertyInfo.Name), true);
                }

                //If all fails
                else
                {
                    throw new PropertyCantBeAutomappedException(string.Format("Property '{0}' of class '{1}' can't be automapped to type '{2}' property '{3}' because their types are incompatible.", myProperty.Name, me.GetType().Name, obj.GetType().Name, objPropertyMeta.PropertyInfo.Name));
                }
            }
            return me;
        }
        public static object MapToObjectCustomBase(this object me, object obj)
        {
            //if me == null, we set obj to null too, the caller must assign the result of this method
            if (me == null) return null;

            //if me is null, we need to create an object
            if (obj == null) throw new ReflectionMapperException("MapToObjectCustomBase(): obj is null");

            //ICollection<ICustomeMapper>
            if (AreCompatibleCollections(me.GetType(), obj.GetType()))
            {
                var myICollection = (ICollection)me;
                var objICollectionGenericArg = obj.GetType().GetInterface(typeof(IEnumerable<>).Name).GetGenericArguments()[0];
                foreach (var myItemObj in myICollection)
                {
                    obj.ExecuteMethod("Add", myItemObj != null ? myItemObj.ExecuteMethod("MapToObjectCustom", ReflectionHelper.CreateType(objICollectionGenericArg), objICollectionGenericArg) : null);
                }
                return me;
            }
            
            foreach (var property in me.GetType().GetProperties())
            {
                //If property is marked NotReflectionMappedAttribute, don't worry about this one
                if (Attribute.GetCustomAttribute(property, typeof(NotRMappedAttribute), true) != null) continue;

                //Find matching properties
                var myProperty = property;
                var objPropertyMeta = GetMatchingDomainProperty(me, myProperty, obj);

                //ICustomMapper
                if (myProperty.PropertyType.GetInterface(typeof(ICustomMapper).Name) != null)
                {
                    try
                    {
                        // ReSharper disable PossibleNullReferenceException
                        var propertyValue = (me.PropertyGet(myProperty.Name) as ICustomMapper).MapToObjectCustom(objPropertyMeta.Obj.PropertyGet(objPropertyMeta.PropertyInfo.Name), objPropertyMeta.PropertyInfo.PropertyType);
                        // ReSharper restore PossibleNullReferenceException
                        objPropertyMeta.Obj.PropertySet(objPropertyMeta.PropertyInfo.Name, propertyValue);
                    }
                    catch (Exception ex)
                    {
                        throw new PropertyCantBeAutomappedException(string.Format("Property '{0}' of class '{1}' can't be automapped to type '{2}' property '{3}' because ICsutomMapper threw an exception: {4}.", myProperty.Name, me.GetType().Name, obj.GetType().Name, objPropertyMeta.PropertyInfo.Name, ex.Message));
                    }
                }

                //IEnumerable<ICustomMapper>
                else if (AreCompatibleCollections(myProperty.PropertyType, objPropertyMeta.PropertyInfo.PropertyType))
                {
                    var myIEnumerable = (IEnumerable)me.PropertyGet(myProperty.Name);
                    var objIEnumerableGenericArg = objPropertyMeta.PropertyInfo.PropertyType.GetInterface(typeof(IEnumerable<>).Name).GetGenericArguments()[0];
                    if (myIEnumerable != null)
                    {
                        objPropertyMeta.Obj.PropertySet(objPropertyMeta.PropertyInfo.Name, ReflectionHelper.CreateGenericType(myIEnumerable.GetType().GetGenericTypeDefinition(), objIEnumerableGenericArg));
                        foreach (var myItemObj in myIEnumerable)
                        {
                            objPropertyMeta.Obj.PropertyGet(myProperty.Name).ExecuteMethod("Add", (myItemObj != null) ? myItemObj.ExecuteMethod("MapToObjectCustom", ReflectionHelper.CreateType(objIEnumerableGenericArg), objIEnumerableGenericArg) : null);
                            //obj.PropertyGet(myProperty.Name).ExecuteMethod("Add", (myItemObj != null) ? ReflectionHelper.CreateType(objIEnumerableGenericArg).ExecuteMethod("MapToObjectCustom", myItemObj, myItemObj.GetType()) : null);
                        }
                    }
                }

                //This is the default case of when the types match exactly
                else if (myProperty.PropertyType == objPropertyMeta.PropertyInfo.PropertyType)
                {
                    objPropertyMeta.Obj.PropertySet(objPropertyMeta.PropertyInfo.Name, me.PropertyGet(myProperty.Name), true);
                }

                //If all fails
                else
                {
                    throw new PropertyCantBeAutomappedException(string.Format("Property '{0}' of class '{1}' can't be automapped to type '{2}' property '{3}' because their types are incompatible.", myProperty.Name, me.GetType().Name, obj.GetType().Name, objPropertyMeta.PropertyInfo.Name));
                }
            }

            return obj;
        }

        #region Private Helper Methods
        private static bool AreCompatibleCollections(Type activeType, Type passiveType)
        {
            //both types are generic
            if (!activeType.IsGenericType) return false;
            if (!passiveType.IsGenericType) return false;

            //get generic type defintions
            var activeTypeGenericDefinition = activeType.GetGenericTypeDefinition();
            var passiveTypeGenericDefinition = passiveType.GetGenericTypeDefinition();

            //make sure the types are the same except for the generic parameter
            if (activeTypeGenericDefinition != passiveTypeGenericDefinition) return false;

            //set up ICollection inetrafce in a variable
            var activeICollectionInterface = activeType.GetInterface(typeof(ICollection<>).Name);
            if (activeICollectionInterface == null && activeTypeGenericDefinition == typeof(ICollection<>)) activeICollectionInterface = activeType;

            var passiveICollectionInterface = passiveType.GetInterface(typeof(ICollection<>).Name);
            if (passiveICollectionInterface == null && passiveTypeGenericDefinition == typeof(ICollection<>)) passiveICollectionInterface = passiveType;
            
            //both types are ICollection
            if (activeICollectionInterface == null) return false;
            if (passiveICollectionInterface == null) return false;

            //both are generic types (this might be generic but just in case)
            if (!activeICollectionInterface.IsGenericType) return false;
            if (!passiveICollectionInterface.IsGenericType) return false;

            //my active generic ICollection<> type argument implements ICustomMapper
            if (activeICollectionInterface.GetGenericArguments()[0].GetInterface(typeof(ICustomMapper).Name) == null) return false;

            return true;
        }
        private static ReflectionMapperPropertyMetadata GetMatchingDomainProperty(object me, PropertyInfo myProperty, object obj)
        {
            // ReSharper disable PossibleMultipleEnumeration
            // ReSharper disable AccessToForEachVariableInClosure

            var myPropertyName = myProperty.Name;
            var parentObj = obj;

            var myReflectionMappedToAttribute = Attribute.GetCustomAttribute(myProperty, typeof(RMapToAttribute), true);
            if (myReflectionMappedToAttribute != null)
            {
                var attrPropertyName = ((RMapToAttribute)myReflectionMappedToAttribute).PropertyName;
                if (!string.IsNullOrEmpty(attrPropertyName)) myPropertyName = attrPropertyName;
                
                var attrObjectPath = ((RMapToAttribute) myReflectionMappedToAttribute).ObjectPath;
                if (!string.IsNullOrEmpty(attrObjectPath))
                {
                    var mappedToNameComponents = attrObjectPath.Split('.');
                    if (mappedToNameComponents.Length < 1) throw new ReflectionMapperException("Invalid path in ReflectionMappedTo Attribute");

                    foreach (var subPropertyName in mappedToNameComponents)
                    {
                        var subObjProperties = parentObj.GetType().GetProperties().Where(x => x.Name == subPropertyName);
                        if (subObjProperties.Count() != 1) throw new ReflectionMapperException("Invalid path in ReflectionMappedTo Attribute");
                        parentObj = parentObj.PropertyGet(subObjProperties.Single().Name);
                        if (parentObj == null) throw new ReflectionMapperException(string.Format("{0} in ReflectionMappedTo Attribute path is null", subPropertyName));
                    }
                }
            }
            
            var objProperties = parentObj.GetType().GetProperties().Where(x => x.Name == myPropertyName);
            if (objProperties.Count() != 1) throw new PropertyCantBeAutomappedException(string.Format("Property '{0}' of class '{1}' can't be automapped to type '{2}' because '{3}' property does not exist in type '{2}'.", myProperty.Name, me.GetType().Name, parentObj.GetType().Name, myPropertyName));
            PropertyInfo propertyInfo = objProperties.Single();

            return new ReflectionMapperPropertyMetadata { Obj = parentObj, PropertyInfo = propertyInfo };

            // ReSharper restore AccessToForEachVariableInClosure
            // ReSharper restore PossibleMultipleEnumeration
        }
        #endregion
    }
}
