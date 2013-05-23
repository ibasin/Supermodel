using System;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class DropdownMvcModelUsingEnum<EnumT> : Mvc.UIComponents.DropdownMvcModelUsingEnum<EnumT> where EnumT : struct, IConvertible {}
    }
}

