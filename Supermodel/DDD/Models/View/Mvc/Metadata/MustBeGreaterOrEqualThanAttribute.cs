using System.ComponentModel.DataAnnotations;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    public class MustBeGreaterOrEqualThanAttribute : ComparisonAttribute
    {
        public MustBeGreaterOrEqualThanAttribute(string match) : base(match) { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var compResult = GetComparisonResult(value, validationContext);
            if (compResult >= 0) return ValidationResult.Success;
            return new ValidationResult(ErrorMessage);
        }
    }
}