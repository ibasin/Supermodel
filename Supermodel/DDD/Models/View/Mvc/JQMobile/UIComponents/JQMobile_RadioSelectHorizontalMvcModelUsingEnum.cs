﻿using System;
using System.Web.Mvc;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class RadioSelectHorizontalMvcModelUsingEnum<EnumT> : Mvc.UIComponents.RadioSelectMvcModelUsingEnum<EnumT> where EnumT : struct, IConvertible
        {
            public override MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return RadioSelectHorizontalMvcModel.RadioSelectCommonEditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute);
            }
        }
    }
}