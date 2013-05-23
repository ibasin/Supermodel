using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ReflectionMapper
{
    public static class ReflectionHelper
    {
        #region Methods
        public static string GetThrowingContext()
        {
            var stackFrame = new StackTrace().GetFrame(2);
            return stackFrame.GetMethod().DeclaringType + "::" + stackFrame.GetMethod().Name + "()";
        }
        public static string GetCurrentContext()
        {
            var stackFrame = new StackTrace().GetFrame(1);
            return stackFrame.GetMethod().DeclaringType + "::" + stackFrame.GetMethod().Name + "()";
        }
        public static object CreateType(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }
        public static object CreateGenericType(Type genericType, Type[] innerTypes, params object[] args)
        {
            Type specificType = genericType.MakeGenericType(innerTypes);
            return Activator.CreateInstance(specificType, args);
        }
        public static object CreateGenericType(Type genericType, Type innerType, params object[] args)
        {
            return CreateGenericType(genericType, new[] {innerType}, args);
        }
        public static object ExecuteStaticMethod(Type typeofClassWithStaticMethod, string methodName, params object[] args)
        {
            var methodInfo = typeofClassWithStaticMethod.GetMethods(BindingFlags.Static | BindingFlags.Public).Single(m => m.Name == methodName && !m.IsGenericMethod);
            return methodInfo.Invoke(null, args);
        }
        
        public static object ExecuteNonPublicStaticMethod(Type typeofClassWithStaticMethod, string methodName, params object[] args)
        {
            var methodInfo = typeofClassWithStaticMethod.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(m => m.Name == methodName && !m.IsGenericMethod);
            return methodInfo.Invoke(null, args);
        }
        
        public static object ExecuteStaticGenericMethod(Type typeofClassWithGenericStaticMethod, string methodName, Type[] genericArguments, params object[] args)
        {
            var methodInfo = typeofClassWithGenericStaticMethod.GetMethods(BindingFlags.Static | BindingFlags.Public).Single(m => m.Name == methodName && m.IsGenericMethod);
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            return genericMethodInfo.Invoke(null, args);
        }

        public static object ExecuteNonPublicStaticGenericMethod(Type typeofClassWithGenericStaticMethod, string methodName, Type[] genericArguments, params object[] args)
        {
            var methodInfo = typeofClassWithGenericStaticMethod.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Single(m => m.Name == methodName && m.IsGenericMethod);
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
            return genericMethodInfo.Invoke(null, args);
        }

        public static bool IsClassADerivedFromClassB(Type a, Type b)
        {
            return IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(a, b) != null;
        }
        public static Type IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(Type a, Type b)
        {
            if (a == b) return a;
            var aBaseType = a.BaseType;
            if (aBaseType == null) return null;

            if (b.IsGenericTypeDefinition && aBaseType.IsGenericType)
            {
                if (aBaseType.GetGenericTypeDefinition() == b) return aBaseType;
            }
            else
            {
                if (aBaseType == b) return aBaseType;
            }
            return IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(aBaseType, b);
        }
        #endregion

        #region Extnesions for Fluent interface
        public static string InsertSpacesBetweenWords(this string str)
        {
            var result = Regex.Replace(str, @"(\B[A-Z][^A-Z]+)|\B(?<=[^A-Z]+)([A-Z]+)(?![^A-Z])", " $1$2");
            return result
                .Replace(" Or ", " or ")
                .Replace(" And ", " and ")
                .Replace(" Of ", " of ")
                .Replace(" On ", " on ")
                .Replace(" The ", " the ")
                .Replace(" For ", " for ")
                .Replace(" At ", " at ")
                .Replace(" A ", " a ")
                .Replace(" In ", " in ")
                .Replace(" By ", " by ")
                .Replace(" About ", " about ")
                .Replace(" To ", " to ")
                .Replace(" From ", " from ")
                .Replace(" With ", " with ")
                .Replace(" Over ", " over ")
                .Replace(" Into ", " into ")
                .Replace(" Without ", " without ");
        }

        public static string GetTypeDescription(this Type type)
        {
            var attr = Attribute.GetCustomAttribute(type, typeof(DescriptionAttribute), true);
            return attr != null ? ((DescriptionAttribute)attr).Description : type.ToString().InsertSpacesBetweenWords();
        }
        public static string GetTypeFriendlyDescription(this Type type)
        {
            var attr = Attribute.GetCustomAttribute(type, typeof(DescriptionAttribute), true);
            return attr != null ? ((DescriptionAttribute)attr).Description : type.Name.InsertSpacesBetweenWords();
        }
        public static string GetDisplayNameForProperty(this Type type, string propertyName)
        {
            var propertyInfo = type.GetProperty(propertyName);
            var attr = propertyInfo.GetCustomAttribute(typeof (DisplayNameAttribute), true);
            return attr != null ? ((DisplayNameAttribute)attr).DisplayName : propertyName.InsertSpacesBetweenWords();
        }
        public static object ExecuteGenericMethod(this object me, string methodName, Type[] genericArguments, params object[] args)
        {
            try
            {
                var methodInfo = me.GetType().GetMethods().Single(x => x.Name == methodName && x.IsGenericMethod);
                var genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
                return genericMethodInfo.Invoke(me, args);
            }
            catch (Exception)
            {
                throw new ReflectionMethodCantBeInvoked(me.GetType(), methodName);
            }
        }
        public static object ExecuteNonPublicGenericMethod(this object me, string methodName, Type[] genericArguments, params object[] args)
        {
            try
            {
                var methodInfo = me.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.Name == methodName && x.IsGenericMethod);
                var genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
                return genericMethodInfo.Invoke(me, args);
            }
            catch (Exception)
            {
                throw new ReflectionMethodCantBeInvoked(me.GetType(), methodName);
            }
        }
        public static object ExecuteMethod(this object me, string methodName, params object[] args)
        {
            var method = me.GetType().GetMethods().SingleOrDefault(x => x.Name == methodName);
            if (method == null)
            {
                throw new ReflectionMethodCantBeInvoked(me.GetType(), methodName);
            }
            return method.Invoke(me, args);
        }
        public static object ExecuteNonPublicMethod(this object me, string methodName, params object[] args)
        {
            try
            {
                return me.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.Name == methodName).Invoke(me, args);
            }
            catch (Exception)
            {
                throw new ReflectionMethodCantBeInvoked(me.GetType(), methodName);
            }
        }

        public static object PropertyGet(this object me, string propertyName, object[] index = null)
        {
            return me.GetProperty(propertyName).GetValue(me, index);
        }
        public static object PropertyGetNonPublic(this object me, string propertyName, object[] index = null)
        {
            return me.GetPropertyNonPublic(propertyName).GetValue(me, index);
        }
        public static void PropertySet(this object me, string propertyName, object newValue, bool ignoreNoSetMethod = false, object[] index = null)
        {
            var myProperty = me.GetProperty(propertyName);
            var setMethod = myProperty.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(me, new[] { newValue });
            }
            else
            {
                if (!ignoreNoSetMethod) myProperty.SetValue(me, newValue, index);
            }
        }
        public static void PropertySetNonPublic(this object me, string propertyName, object newValue, bool ignoreNoSetMethod = false, object[] index = null)
        {
            var myProperty = me.GetPropertyNonPublic(propertyName);
            if (ignoreNoSetMethod)
            {
                if (myProperty.GetSetMethod() != null) myProperty.SetValue(me, newValue, index);
            }
            else
            {
                myProperty.SetValue(me, newValue, index);
            }
        }
        #endregion

        #region Private Helpers
        private static PropertyInfo GetPropertyNonPublic(this object me, string propertyName)
        {
            try
            {
                return me.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(p => p.Name == propertyName);
            }
            catch (Exception)
            {
                throw new ReflectionPropertyCantBeInvoked(me.GetType(), propertyName);
            }
        } 

        private static PropertyInfo GetProperty(this object me, string propertyName)
        {
            try
            {
                return me.GetType().GetProperty(propertyName);
            }
            catch(Exception)
            {
                throw new ReflectionPropertyCantBeInvoked(me.GetType(), propertyName);
            }
        } 
        #endregion
    }
}
