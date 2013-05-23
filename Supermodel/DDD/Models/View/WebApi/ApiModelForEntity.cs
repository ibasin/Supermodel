using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.View.WebApi
{
    abstract public class ApiModelForEntity<EntityT> : IViewModelForEntity<EntityT> where EntityT : class, IEntity, new()
    {
        #region Methods
        [NotRMapped, ScaffoldColumn(false), JsonIgnore] public bool IsNewModel { get { return Id == 0; } }
        #endregion
        
        #region ICustom mapper implementation
        public virtual object MapFromObjectCustom(object obj, Type objType)
        {
            return this.MapFromObjectCustomBase(obj);
        }

        public virtual object MapToObjectCustom(object obj, Type objType)
        {
            return this.MapToObjectCustomBase(obj);
        }
        #endregion

        #region Standard Properties for Web API Models
        public virtual int Id { get; set; }
        #endregion

        #region Validation
        //The default implemetation just grabs domain model validation but this can be overriden
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return CreateTempEntity().Validate(validationContext);
        }
        #endregion

        #region Private Helper Methods
        private EntityT CreateTempEntity()
        {
            return (EntityT)this.MapToObject(new EntityT());
        }
        #endregion
    }
}