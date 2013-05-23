using System;

namespace ReflectionMapper
{
    public class ReflectionMapperException : Exception
    {
        public ReflectionMapperException() { }
        public ReflectionMapperException(string msg) : base(msg) { }
    }

    public class ReflectionPropertyCantBeInvoked : ReflectionMapperException
    {
        public ReflectionPropertyCantBeInvoked(Type type, string propertyName)
            : base(string.Format("Property '{0}' does not exist in Type '{1}'", propertyName, type.Name)) { }
    }

    public class ReflectionMethodCantBeInvoked : ReflectionMapperException
    {
        public ReflectionMethodCantBeInvoked(Type type, string methodName)
            : base(string.Format("Method '{0}' does not exist in Type '{1}'", methodName, type.Name)) { }
    }    
    
    public class PropertyCantBeAutomappedException : ReflectionMapperException
    {
        public PropertyCantBeAutomappedException(string msg) : base(msg) { }
    }
}
