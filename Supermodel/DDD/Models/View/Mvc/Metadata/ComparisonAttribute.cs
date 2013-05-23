using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ComparisonAttribute : ValidationAttribute
    {
        public String MatchProperty { get; set; }

        protected ComparisonAttribute(string match)
        {
            MatchProperty = match;
        }

        protected int GetComparisonResult(object value, ValidationContext validationContext)
        {
            var objectType = validationContext.ObjectType;
            object matchValue;

            var linqMatches = objectType.GetProperties().Where(propertyInfo => propertyInfo.Name == MatchProperty);
            // ReSharper disable PossibleMultipleEnumeration
            if (linqMatches.Any())
            {
                var propertyInfo = linqMatches.First();
                matchValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
            }
            else
            {
                throw new SupermodelException("Unable to find match to compare with");
            }
            // ReSharper restore PossibleMultipleEnumeration

            var comparableValue = value as IComparable;
            var comparableMatchValue = matchValue as IComparable;
            if (comparableValue == null || comparableMatchValue == null) throw new SupermodelException("Comparision attributes are only applicable to types implementing IComparable");
            return comparableValue.CompareTo(comparableMatchValue);
        }
    }
}