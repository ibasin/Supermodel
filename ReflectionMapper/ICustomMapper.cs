using System;

namespace ReflectionMapper
{
    /// <summary>
    /// A class that knows how to map itself to and from an underlying object; 
    /// used by SuperModel's DefaultModelBinder.
    /// </summary>
    public interface ICustomMapper
    {
        //must also have a default constructor!
        object MapFromObjectCustom(object obj, Type objType);
        object MapToObjectCustom(object obj, Type objType);    
    }
}
