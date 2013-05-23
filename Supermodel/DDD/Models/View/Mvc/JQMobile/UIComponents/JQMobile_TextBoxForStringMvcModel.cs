using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class TextBoxForStringMvcModel : ICustomMapper, IMvcModelEditorTemplate, IMvcModelDisplayTemplate, IMvcModelModelBinder, IComparable
        {
            #region ICustomMapper implemtation
            public virtual object MapFromObjectCustom(object obj, Type objType)
            {
                if (objType != typeof(string)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

                var domainObj = (string)obj;
                Value = domainObj;

                return this;
            }
            public virtual object MapToObjectCustom(object obj, Type objType)
            {
                if (objType != typeof(string)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

                return Value;
            }
            #endregion

            #region IMvcModelEditorTemplate implemtation
            public virtual bool GetIEditorTemplateImplemented() { return true; }
            public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                var htmlAttributes = new Dictionary<string, object> { { "data-clear-btn", true } };
                if (Placeholder != null) htmlAttributes.Add("placeholder", Placeholder);
                return html.TextBox("", Value ?? "", htmlAttributes);
            }
            #endregion

            #region IMvcModelDisplayTemplate
            public virtual bool GetIDisplayTemplateImplemented() { return true; }
            public virtual MvcHtmlString DisplayTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
            {
                return MvcHtmlString.Create(Value ?? "");
            }
            #endregion

            #region IMvcModelModelBinder implemtation
            public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                string key = bindingContext.ModelName;
                ValueProviderResult val = bindingContext.ValueProvider.GetValue(key);
                string attemptedValue;
                if (val == null || string.IsNullOrEmpty(val.AttemptedValue))
                {
                    if (bindingContext.ModelMetadata.IsRequired) bindingContext.ModelState.AddModelError(key, string.Format("The field {0} is required", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                    // ReSharper disable RedundantAssignment
                    attemptedValue = "";
                    // ReSharper restore RedundantAssignment
                    Value = null;
                }
                else
                {
                    attemptedValue = val.AttemptedValue;
                    try
                    {
                        Value = attemptedValue;
                    }
                    catch (FormatException)
                    {
                        Value = null;
                        bindingContext.ModelState.AddModelError(key, string.Format("The field {0} is invalid", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                    }
                }

                bindingContext.ModelState.SetModelValue(key, val);

                var existingModel = (TextBoxForStringMvcModel)bindingContext.Model;
                if (existingModel != null)
                {
                    existingModel.Value = Value;
                    return existingModel;
                }
                return this;
            }
            #endregion

            #region IComparable implemetation
            public int CompareTo(object obj)
            {
                var valueToCompareWith = ((TextBoxForStringMvcModel)obj).Value;
                if (Value == null && valueToCompareWith == null) return 0;
                if (Value == null || valueToCompareWith == null) return 1;
                return String.Compare((Value), valueToCompareWith, StringComparison.InvariantCulture);
            }
            #endregion

            #region Properies
            public string Value { get; set; }
            public string Placeholder { get; set; }
            #endregion

        }
    }
}
