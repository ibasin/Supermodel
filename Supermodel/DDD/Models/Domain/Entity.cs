using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReflectionMapper;
using Supermodel.DDD.Models.Validation;
using Supermodel.DDD.Repository;

namespace Supermodel.DDD.Models.Domain
{
    public abstract class Entity : IEntity
    {
        #region Standard Properties
        public virtual int Id { get; protected set; }
        #endregion

        #region Methods

        public virtual IEntity ConstructVirtualProperties()
        {
            return this;
        }

        public virtual void Add()
        {
            var originalId = Id;
            try
            {
                AddInternal();
            }
            catch (Exception)
            {
                Id = originalId;
                throw;
            }
        }
        public virtual void Delete()
        {
            var originalId = Id;
            try
            {
                DeleteInternal();
            }
            catch (Exception)
            {
                Id = originalId;
                throw;
            }
        }
        public bool IsNewModel { get { return this.Id == 0; } }
        #endregion

        #region Overridable Methods
        protected virtual void AddInternal()
        {
            CreateRepo().ExecuteMethod("Add", this);
        }
        protected virtual void DeleteInternal()
        {
            CreateRepo().ExecuteMethod("Delete", this);
        }
        protected virtual object CreateRepo()
        {
            return RepoFactory.CreateForRuntimeType(GetDbSetTypeOfPossibleProxy(GetType()));
        }
        #endregion

        #region Validation
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new ValidationResultList();
        }
        #endregion

        #region Static Methods
        public static Type GetDbSetTypeOfPossibleProxy(Type dataSetType)
        {
            if (dataSetType == null) throw new ArgumentNullException("dataSetType");
            // ReSharper disable PossibleNullReferenceException
            if (dataSetType.FullName.StartsWith("System.Data.Entity.DynamicProxies.")) dataSetType = dataSetType.BaseType;
            // ReSharper restore PossibleNullReferenceException
            return dataSetType;
        }
        public Type GetNonProxyType()
        {
            return GetDbSetTypeOfPossibleProxy(this.GetType());
        }
        #endregion
    }
}
