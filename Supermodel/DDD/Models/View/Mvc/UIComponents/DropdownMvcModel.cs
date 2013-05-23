using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using ReflectionMapper;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class DropdownMvcModel  : IComparable, IMvcModelModelBinder, IMvcModelEditorTemplate
    {
        public class Option
        {
            public Option(string value, string label, bool isDisabled = false)
            {
                Value = value;
                Label = label;
                IsDisabled = isDisabled;
            }
            public string Value { get; private set; }
            public string Label { get; private set; }
            public bool IsDisabled { get; private set; }
        }
        // ReSharper disable InconsistentNaming
        public List<Option> Options = new List<Option>();
        // ReSharper restore InconsistentNaming

        public string SelectedValue { get; set; }
        public string SelectedLabel
        {
            get
            {
                var selectedOption = Options.FirstOrDefault(x => x.Value == SelectedValue);
                return selectedOption != null ? selectedOption.Label : null;
            }
        }
        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(SelectedValue); }
        }

        public List<SelectListItem> GetSelectListItemList()
        {
            var selectListItemList = new List<SelectListItem> { new SelectListItem { Value = "", Text = "" } };
            foreach (var option in Options)
            {
                var isSelectedOption = (SelectedValue != null && string.CompareOrdinal(SelectedValue, option.Value) == 0);
                if (isSelectedOption || !option.IsDisabled)
                {
                    var item = new SelectListItem {Value = option.Value, Text = !option.IsDisabled ? option.Label : option.Label + " [DISABLED]", Selected = isSelectedOption};
                    selectListItemList.Add(item);
                }
            }
            return selectListItemList;
        }
        public override string ToString()
        {
            return SelectedValue;
        }
        public int CompareTo(object obj)
        {
            return string.CompareOrdinal(SelectedValue, ((DropdownMvcModel)obj).SelectedValue);
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
            string attemptedValue;
            if (val == null || string.IsNullOrEmpty(val.AttemptedValue))
            {
                if (bindingContext.ModelMetadata.IsRequired) bindingContext.ModelState.AddModelError(key, string.Format("The {0} field is required", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                attemptedValue = "";
            }
            else
            {
                attemptedValue = val.AttemptedValue;
            }

            bindingContext.ModelState.SetModelValue(key, val);

            SelectedValue = attemptedValue;

            var existingModel = (DropdownMvcModel)bindingContext.Model;
            if (existingModel != null)
            {
                existingModel.SelectedValue = SelectedValue;
                return existingModel;
            }
            return this;
        }

        public virtual bool GetIEditorTemplateImplemented() { return true; }
        public virtual MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null)
        {
            if (html.ViewData.Model == null) throw new NullReferenceException(ReflectionHelper.GetCurrentContext() + "is called for a model that is null");
            if (!(html.ViewData.Model is DropdownMvcModel)) throw new InvalidCastException(ReflectionHelper.GetCurrentContext() + " is called for a model of type diffrent from DropdownMvcModel.");

            var dropdown = (DropdownMvcModel)html.ViewData.Model;
            return html.DropDownList("", dropdown.GetSelectListItemList());    
        }
    }
}