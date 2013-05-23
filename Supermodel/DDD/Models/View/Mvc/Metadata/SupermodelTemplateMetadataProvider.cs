using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ReflectionMapper;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    public class SupermodelTemplateMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            if (metadata.DisplayName == null && propertyName != null)
            {
                var calculatedDisplayName = propertyName.InsertSpacesBetweenWords();
                if (calculatedDisplayName != propertyName) metadata.DisplayName = calculatedDisplayName;
            }
            
            // ReSharper disable PossibleMultipleEnumeration
            var additionalForceRequiredLabelValues = attributes.OfType<ForceRequiredLabelAttribute>().FirstOrDefault();
            if (additionalForceRequiredLabelValues != null) metadata.AdditionalValues.Add("ForceRequiredLabel", additionalForceRequiredLabelValues);

            var additionalNoRequiredLabelValues = attributes.OfType<NoRequiredLabelAttribute>().FirstOrDefault();
            if (additionalNoRequiredLabelValues != null) metadata.AdditionalValues.Add("NoRequiredLabel", additionalNoRequiredLabelValues);

            var additionalDisplayOnlyValues = attributes.OfType<DisplayOnlyAttribute>().FirstOrDefault();
            if (additionalDisplayOnlyValues != null) metadata.AdditionalValues.Add("DisplayOnly", additionalDisplayOnlyValues);

            var additionalScreenOrderValues = attributes.OfType<ScreenOrderAttribute>().FirstOrDefault();
            if (additionalScreenOrderValues != null) metadata.AdditionalValues.Add("ScreenOrder", additionalScreenOrderValues);

            var additionalHtmlAttrValues = attributes.OfType<HtmlAttrAttribute>().FirstOrDefault();
            if (additionalHtmlAttrValues != null) metadata.AdditionalValues.Add("HtmlAttr", additionalHtmlAttrValues);

            var additionalHideLabelAttrValues = attributes.OfType<HideLabelAttribute>().FirstOrDefault();
            if (additionalHideLabelAttrValues != null) metadata.AdditionalValues.Add("HideLabel", additionalHideLabelAttrValues);
            // ReSharper restore PossibleMultipleEnumeration
            
            return metadata;
        }
    }
}