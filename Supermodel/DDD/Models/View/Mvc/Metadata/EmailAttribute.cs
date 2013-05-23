using System.ComponentModel.DataAnnotations;

namespace Supermodel.DDD.Models.View.Mvc.Metadata
{
    public class EmailAttribute : RegularExpressionAttribute 
    {
        public EmailAttribute() : base( RegexHelper.EmailRegex ) {}
    }
}
