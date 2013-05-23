using System;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HtmlAttrAttribute : Attribute
    {
        public HtmlAttrAttribute(string attr)
        {
            Attr = attr;
        }

        public string Attr { get; set; }
    }
}