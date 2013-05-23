using System;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ScreenOrderAttribute : Attribute 
    {
        public ScreenOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; set; }
    }
}