using System;

namespace ReflectionMapper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotRMappedAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RMapToAttribute : Attribute
    {
        public RMapToAttribute(){}
        public RMapToAttribute(string objectPath)
        {
            ObjectPath = objectPath;
        }
        public RMapToAttribute(string objectPath, string propertyName) : this(objectPath)
        {
            PropertyName = propertyName;
        }
        public string ObjectPath { get; set; }
        public string PropertyName { get; set; }
    }    
}
