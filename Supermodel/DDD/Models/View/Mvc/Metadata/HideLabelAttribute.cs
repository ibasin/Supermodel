using System;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideLabelAttribute : Attribute { }
}
