using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ReflectionMapper;
using Supermodel.DDD.Models.View.Mvc.UIComponents;

namespace Supermodel.DDD.Models.View.Mvc
{
    abstract public class MvcModelForEntityCore : MvcModel, ICustomMapper
    {
        #region Methods
        [NotRMapped, ScaffoldColumn(false)] public bool IsNewModel { get { return Id == 0; } }
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

        #region Standard Properties for Mvc Models
        [ScaffoldColumn(false)]
        public virtual int Id { get; set; }

        [ScaffoldColumn(false), NotRMapped]
        public abstract string Label { get; }

        [ScaffoldColumn(false), NotRMapped]
        public virtual bool IsDisabled { get { return false; } }

        [ScaffoldColumn(false), NotRMapped]
        public virtual bool ContainsBinaryFileProperties
        {
            get { return this.GetType().GetProperties().Any(property => property.PropertyType == typeof(BinaryFileMvcModel)); }
        }
        #endregion
    }
}