using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class DateMvcModel : ICustomMapper, IMvcModelEditorTemplate, IMvcModelDisplayTemplate, IMvcModelModelBinder, IComparable
    {
        #region Constructors
        public DateMvcModel()
        {
            Value = null;
        }
        #endregion

        #region ICustomMapper implemtation
        public virtual object MapFromObjectCustom(object obj, Type objType)
        {
            if (objType != typeof(DateTime) && objType != typeof(DateTime?)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            var domainObj = (DateTime?) obj;
            Value = domainObj;

            return this;
        }
        public virtual object MapToObjectCustom(object obj, Type objType)
        {
            if (objType != typeof(DateTime) && objType != typeof(DateTime?)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

            return Value;
        }
        #endregion

        #region IMvcModelEditorTemplate implemtation
        public virtual bool GetIEditorTemplateImplemented() { return true; }
        public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            //var dateTimeStr = (Value == null) ? "" : ((DateTime)Value).ToString("yyyy-MM-dd");
            var dateTimeStr = (Value == null) ? "" : ((DateTime) Value).ToShortDateString();
            return html.TextBox("", dateTimeStr, new Dictionary<string, object> { { "data-sm-DatePicker", true } });
        }
        #endregion

        #region IMvcModelDisplayTemplate
        public virtual bool GetIDisplayTemplateImplemented() { return true; }
        public virtual MvcHtmlString DisplayTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            var dateTimeStr = (Value == null) ? "" : ((DateTime)Value).ToShortDateString();
            return MvcHtmlString.Create(dateTimeStr);
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
                    Value = DateTime.Parse(attemptedValue);
                }
                catch(FormatException)
                {
                    Value = null;
                    bindingContext.ModelState.AddModelError(key, string.Format("The field {0} is invalid", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                }
            }

            bindingContext.ModelState.SetModelValue(key, val);

            var existingModel = (DateMvcModel)bindingContext.Model;
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
            var valueToCompareWith = ((DateMvcModel)obj).Value;
            if (Value == null && valueToCompareWith == null) return 0;
            if (Value == null || valueToCompareWith == null) return 1;
            return ((DateTime) Value).CompareTo(valueToCompareWith);
        }
        #endregion

        #region Properties
        public DateTime? Value { get; set; }
        #endregion

    }

    public static class MvcModelDateTimeExtensions
    {
        public static DateTime? GetValue(this DateMvcModel me)
        {
            return me.Value;
        }
        public static DateMvcModel SetValue(this DateMvcModel me, DateTime? value)
        {
            me.Value = value;
            return me;
        }
    }
}
