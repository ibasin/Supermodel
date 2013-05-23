using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;

namespace Supermodel.DDD.Models.View.Mvc.Mobile.UIComponents
{
    public abstract class TextBoxForNumberMvcModelBase<NumT> : ICustomMapper, IMvcModelEditorTemplate, IMvcModelDisplayTemplate, IMvcModelModelBinder, IComparable
    {
        #region Constructors
        protected TextBoxForNumberMvcModelBase()
        {
            Pattern = null;
            Step = null;
            Placeholder = null;
        }
        #endregion

        #region ICustomMapper implemtation
        public object MapFromObjectCustom(object obj, Type objType)
        {
            var domainObj = (NumT)obj;
            Value = domainObj;

            return this;
        }
        public object MapToObjectCustom(object obj, Type objType)
        {
            return Value;
        }
        #endregion

        #region IMvcModelEditorTemplate implemtation
        public virtual bool GetIEditorTemplateImplemented() { return true; }
        public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            var htmlAttributes = new Dictionary<string, object> {{"type", "number"}, {"data-clear-btn", true}};
            if (Pattern != null) htmlAttributes.Add("pattern", Pattern);
            if (Step != null) htmlAttributes.Add("step", Step);
            if (Placeholder != null) htmlAttributes.Add("placeholder", Placeholder);
            return html.TextBox("", GetStringValue(), htmlAttributes);
        }
        #endregion
            
        #region IMvcModelDisplayTemplate
        public virtual bool GetIDisplayTemplateImplemented() { return true; }
        public virtual MvcHtmlString DisplayTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            return MvcHtmlString.Create(GetStringValue());
        }
        #endregion

        #region IMvcModelModelBinder implemtation
        public abstract object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext);
        #endregion

        #region IComparable implemetation
        public abstract int CompareTo(object obj);
        #endregion

        #region Protected Helpers
        protected string GetStringValue()
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            return (IsNulableValue() && Value == null) ? "" : Value.ToString();
            // ReSharper restore CompareNonConstrainedGenericWithNull
        }
        protected bool IsNulableValue()
        {
            var type = typeof(NumT);
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>));
        }
        #endregion

        #region Properties
        public NumT Value { get; set; }
        public string Pattern { get; set; }
        public string Step { get; set; }
        public string Placeholder { get; set; }
        public const string IntegerPattern = "[0-9]*";
        #endregion
    }

}
