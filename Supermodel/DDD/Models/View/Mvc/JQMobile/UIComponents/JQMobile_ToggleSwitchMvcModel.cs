using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;
using Supermodel.Mvc.Extensions;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class ToggleSwitchMvcModel : ICustomMapper, IMvcModelEditorTemplate, IMvcModelDisplayTemplate, IMvcModelModelBinder, IComparable
        {
            #region ICustomMapper implemtation
            public object MapFromObjectCustom(object obj, Type objType)
            {
                if (objType != typeof (bool) && objType != typeof (bool?)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

                if (obj != null)
                {
                    var domainObj = (bool) obj;
                    Value = domainObj;
                }
                else
                {
                    Value = false;
                }

                return this;
            }
            public object MapToObjectCustom(object obj, Type objType)
            {
                if (objType != typeof (bool) && objType != typeof (bool?)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));

                return Value;
            }
            #endregion

            #region IMvcModelEditorTemplate implemtation
            public virtual bool GetIEditorTemplateImplemented()
            {
                return true;
            }
            public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = Int32.MinValue, int screenOrderTo = Int32.MaxValue, string markerAttribute = null)
            {
                var optionsList = new List<SelectListItem>
                    {
                        new SelectListItem {Value = "off", Text = "Off", Selected = !Value},
                        new SelectListItem {Value = "on", Text = "On", Selected = Value}
                    };
                return html.DropDownList("", optionsList, new Dictionary<string, object> {{"data-role", "slider"}});
            }

            #endregion

            #region IMvcModelDisplayTemplate
            public virtual bool GetIDisplayTemplateImplemented()
            {
                return true;
            }
            public virtual MvcHtmlString DisplayTemplate(HtmlHelper html, int screenOrderFrom = Int32.MinValue, int screenOrderTo = Int32.MaxValue, string markerAttribute = null)
            {
                return EditorTemplate(html, screenOrderFrom, screenOrderTo, markerAttribute).Supermodel().DisableAllControls();
            }
            #endregion

            #region IMvcModelModelBinder implemtation

            public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                string key = bindingContext.ModelName;
                var val = bindingContext.ValueProvider.GetValue(key);

                //Toggle switch is always requeired, if value is null, we assume false
                if (val == null || string.IsNullOrEmpty(val.AttemptedValue)) Value = false;
                else Value = (val.AttemptedValue.ToLower() == "on");

                bindingContext.ModelState.SetModelValue(key, val);

                var existingModel = (ToggleSwitchMvcModel) bindingContext.Model;
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
                var valueToCompareWith = ((ToggleSwitchMvcModel) obj).Value;
                return Value.CompareTo(valueToCompareWith);
            }

            #endregion

            #region Properies
            public bool Value { get; set; }
            #endregion
        }
    }
}
