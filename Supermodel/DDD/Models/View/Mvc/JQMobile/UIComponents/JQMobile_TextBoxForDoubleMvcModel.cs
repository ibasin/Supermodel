using System;
using System.Web.Mvc;
using Supermodel.DDD.Models.View.Mvc.Mobile.UIComponents;

// ReSharper disable CheckNamespace
namespace Supermodel.DDD.Models.View.Mvc.JQMobile
// ReSharper restore CheckNamespace
{
    public abstract partial class JQMobile
    {
        public class TextBoxForDoubleMvcModel : TextBoxForNumberMvcModelBase<double?>
        {
            #region Constructors
            public TextBoxForDoubleMvcModel()
            {
                Pattern = null;
                Step = "0.001";
            }
            #endregion

            #region IMvcModelModelBinder implemtation
            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
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
                        Value = double.Parse(attemptedValue);
                    }
                    catch (FormatException)
                    {
                        Value = null;
                        bindingContext.ModelState.AddModelError(key, string.Format("The field {0} is invalid", bindingContext.ModelMetadata.DisplayName ?? bindingContext.ModelMetadata.PropertyName));
                    }
                }

                bindingContext.ModelState.SetModelValue(key, val);

                var existingModel = (TextBoxForDoubleMvcModel)bindingContext.Model;
                if (existingModel != null)
                {
                    existingModel.Value = Value;
                    return existingModel;
                }
                return this;
            }
            #endregion

            #region IComparable implemetation
            public override int CompareTo(object obj)
            {
                var valueToCompareWith = ((TextBoxForDoubleMvcModel)obj).Value;
                if (Value == null && valueToCompareWith == null) return 0;
                if (Value == null || valueToCompareWith == null) return 1;
                return ((double)Value).CompareTo((double)valueToCompareWith);
            }
            #endregion
        }
    }
}
