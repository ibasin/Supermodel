using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.View.Mvc
{
    abstract public class MvcModelForEntity<EntityT> : MvcModelForEntityCore, IViewModelForEntity<EntityT> where EntityT : class, IEntity, new()
    {
        #region Controller Properties for Mvc Models
        [ScaffoldColumn(false), NotRMapped]
        public virtual string MyControllerName { get { return typeof (EntityT).Name; } }
        #endregion

        #region Validation
        //The default implemetation just grabs domain model validation but this can be overriden
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return CreateTempValidationEntity().Validate(validationContext);
        }
        #endregion
        
        #region Private Helper Methods
        protected virtual EntityT CreateTempValidationEntity()
        {
            return (EntityT)this.MapToObject(new EntityT().ConstructVirtualProperties());
        }
        #endregion
    }
}