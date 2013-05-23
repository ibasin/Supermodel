using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public abstract class MultiSelectMvcModelCore : IComparable, IMvcModelModelBinder, IMvcModelEditorTemplate
    {
        public class Option
        {
            public Option(string value, string label, bool isDisabled) : this(value, label, isDisabled, false) { }
            public Option(string value, string label, bool isDisabled, bool selected)
            {
                Value = value;
                Label = label;
                IsDisabled = isDisabled;
                Selected = selected;
            }
            public string Value { get; private set; }
            public string Label { get; private set; }
            public bool IsDisabled { get; private set; }
            public bool Selected { get; set; }
            public bool IsShown { get { return Selected || !IsDisabled; } }
        }

        public List<Option> Options = new List<Option>();

        public List<SelectListItem> GetSelectListItemList()
        {
            var selectListItemList = new List<SelectListItem>();
            foreach (var option in Options)
            {
                if (option.IsShown)
                {
                    var item = new SelectListItem { Value = option.Value, Text = !option.IsDisabled ? option.Label : option.Label + " [DISABLED]", Selected = option.Selected };
                    selectListItemList.Add(item);
                }
            }
            return selectListItemList;
        }

        public int CompareTo(object obj)
        {
            var other = (MultiSelectMvcModelCore)obj;
            if (Options.Count != other.Options.Count) return 1;

            foreach (var option in Options)
            {
                // ReSharper disable AccessToModifiedClosure
                if (other.Options.Find(x => x.Value == option.Value && x.Label == option.Label && x.Selected == option.Selected) == null) return 1;
                // ReSharper restore AccessToModifiedClosure
            }
            return 0;
        }

        public virtual bool GetIEditorTemplateImplemented() { return true; }
        public abstract MvcHtmlString EditorTemplate(HtmlHelper html, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, string markerAttribute = null);

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null) throw new ArgumentNullException("controllerContext");
            if (bindingContext == null) throw new ArgumentNullException("bindingContext");

            string key = bindingContext.ModelName;
            ValueProviderResult val = bindingContext.ValueProvider.GetValue(key);
            string attemptedValue;
            if (val == null || string.IsNullOrEmpty(val.AttemptedValue))
            {
                if (bindingContext.ModelMetadata.IsRequired) bindingContext.ModelState.AddModelError(key, string.Format("The field {0} is required", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                attemptedValue = "";
            }
            else
            {
                attemptedValue = val.AttemptedValue;
            }

            bindingContext.ModelState.SetModelValue(key, val);
            var attemptedValues = attemptedValue.Split(',');
            var existingModel = (MultiSelectMvcModelCore)bindingContext.Model;
            if (existingModel != null)
            {
                //Clear out selected
                existingModel.Options.ForEach(x => x.Selected = false);
                foreach (var selectedValue in attemptedValues)
                {
                    // ReSharper disable AccessToModifiedClosure
                    var selectedOption = existingModel.Options.Find(x => x.Value == selectedValue);
                    // ReSharper restore AccessToModifiedClosure
                    if (selectedOption != null) selectedOption.Selected = true;
                }
                return existingModel;
            }

            Options.Clear();
            foreach (var value in attemptedValues) Options.Add(new Option(value, "N/A", false, true));
            return this;
        }
    }
}