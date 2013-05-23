using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ReflectionMapper;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.UIComponents;

namespace Supermodel.Mvc.DefaultModelBinders
{
    public class SupermodelDefaultModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            // need to skip properties that aren't part of the request, else we might hit a StackOverflowException
            var fullPropertyKey = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
            if (!bindingContext.ValueProvider.ContainsPrefix(fullPropertyKey))
            {
                /*****Supermodel custom code***********/
                //CheckSpecialCasesForRequired(controllerContext, bindingContext, propertyDescriptor, null);
                /*****End Supermodel custom code*******/
                return;
            }

            // call into the property's model binder
            /*****Supermodel custom code***********/
            IModelBinder propertyBinder;
            if (propertyDescriptor.PropertyType.GetInterface(typeof(IMvcModelModelBinder).Name) != null)
            {
                propertyBinder = (IModelBinder)ReflectionHelper.CreateType(propertyDescriptor.PropertyType);
            }
            else
            {
                propertyBinder = Binders.GetBinder(propertyDescriptor.PropertyType);
            }

            /*****End Supermodel custom code*******/
            object originalPropertyValue = propertyDescriptor.GetValue(bindingContext.Model);
            var propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            propertyMetadata.Model = originalPropertyValue;
            var innerBindingContext = new ModelBindingContext
            {
                ModelMetadata = propertyMetadata,
                ModelName = fullPropertyKey,
                ModelState = bindingContext.ModelState,
                ValueProvider = bindingContext.ValueProvider
            };
            object newPropertyValue = GetPropertyValue(controllerContext, innerBindingContext, propertyDescriptor, propertyBinder);
            
            /*****Supermodel custom code***********/
            //BinaryFileMvcModel is a special case
            if (originalPropertyValue is BinaryFileMvcModel)
            {
                if (!((BinaryFileMvcModel)newPropertyValue).IsEmpty) propertyMetadata.Model = newPropertyValue;
                else newPropertyValue = originalPropertyValue;
            }
            else
            {
                propertyMetadata.Model = newPropertyValue;    
            }
            /*****End Supermodel custom code*******/

            // validation
            var modelState = bindingContext.ModelState[fullPropertyKey];
            if (modelState == null || modelState.Errors.Count == 0)
            {
                if (OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, newPropertyValue))
                {
                    SetProperty(controllerContext, bindingContext, propertyDescriptor, newPropertyValue);
                    OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, newPropertyValue);
                }
            }
            else
            {
                SetProperty(controllerContext, bindingContext, propertyDescriptor, newPropertyValue);

                // Convert FormatExceptions (type conversion failures) into InvalidValue messages
                foreach (var error in modelState.Errors.Where(err => String.IsNullOrEmpty(err.ErrorMessage) && err.Exception != null).ToList())
                {
                    for (var exception = error.Exception; exception != null; exception = exception.InnerException)
                    {
                        if (exception is FormatException)
                        {
                            var displayName = propertyMetadata.GetDisplayName();
                            var errorMessageTemplate = GetValueInvalidResource(controllerContext);
                            var errorMessage = String.Format(CultureInfo.CurrentCulture, errorMessageTemplate, modelState.Value.AttemptedValue, displayName);
                            modelState.Errors.Remove(error);
                            modelState.Errors.Add(errorMessage);
                            break;
                        }
                    }
                }
            }
        }
        /*
        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            
            CheckSpecialCasesForRequired(controllerContext, bindingContext, propertyDescriptor, value);
            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }

        private static void CheckSpecialCasesForRequired(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            //this handling is a special case for BinaryFileMvcModel & MvcModelDropDown and its derivatives -- validation for [Required]
            var propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
            propertyMetadata.Model = value;
            var modelStateKey = CreateSubPropertyName(bindingContext.ModelName, propertyMetadata.PropertyName);

            if (propertyDescriptor.PropertyType == typeof(BinaryFileMvcModel) && bindingContext.ModelState.IsValidField(modelStateKey) && (value == null || ((BinaryFileMvcModel)value).IsEmpty))
            {
                ModelValidator requiredValidator = ModelValidatorProviders.Providers.GetValidators(propertyMetadata, controllerContext).Where(v => v.IsRequired).FirstOrDefault();
                if (requiredValidator != null) bindingContext.ModelState.AddModelError(modelStateKey, string.Format("The {0} field is required", propertyDescriptor.DisplayName));
            }
            /*
            if (ReflectionHelpers.IsClassADrivedFromClassB(propertyDescriptor.PropertyType, typeof(DropdownMvcModel)) && bindingContext.ModelState.IsValidField(modelStateKey) && (value == null || ((DropdownMvcModel)value).IsEmpty))
            {
                ModelValidator requiredValidator = ModelValidatorProviders.Providers.GetValidators(propertyMetadata, controllerContext).Where(v => v.IsRequired).FirstOrDefault();
                if (requiredValidator != null) bindingContext.ModelState.AddModelError(modelStateKey, string.Format("The {0} field is required", propertyDescriptor.DisplayName));
            }
            * /
        }
        */
        #region Private Helpers
        private static string GetUserResourceString(ControllerContext controllerContext, string resourceName)
        {
            string result = null;

            if (!String.IsNullOrEmpty(ResourceClassKey) && (controllerContext != null) && (controllerContext.HttpContext != null))
            {
                result = controllerContext.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName, CultureInfo.CurrentUICulture) as string;
            }

            return result;
        }
        private static string GetValueInvalidResource(ControllerContext controllerContext)
        {
            return GetUserResourceString(controllerContext, "PropertyValueInvalid") ?? "The value '{0}' is not valid for {1}";
        }
        #endregion
    }
}
