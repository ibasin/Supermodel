using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.JQMobile;
using Supermodel.DDD.Models.View.Mvc.UIComponents;
using Supermodel.DDD.Repository;

namespace TestDomain.Entities
{
    public class TestChildMobileMvcModel : JQMobile.ChildMvcModelForEntity<TestChild, TestPerson>
    {
        public string Name { get; set; }

        public override string Label
        {
            get { return Name; }
        }
        public override TestPerson GetParentEntity(TestChild entity)
        {
            return entity.ParentPerson;
        }
        public override void SetParentEntity(TestChild entity, TestPerson parent)
        {
            entity.ParentPerson = parent;
        }
    }
    
    public class TestChildMvcModel : ChildMvcModelForEntity<TestChild, TestPerson>
    {
        public string Name { get; set; }
        public BinaryFileMvcModel Image { get; set; }

        public override string Label
        {
            get { return Name; }
        }
        public override TestPerson GetParentEntity(TestChild entity)
        {
            return entity.ParentPerson;
        }
        public override void SetParentEntity(TestChild entity, TestPerson parent)
        {
            entity.ParentPerson = parent;
        }
    }
    
    //This is an example implemetation of an IEntity interface instead of dericing directly from Entity, this is more work but allows to inherit from somehting else
    public class TestChild : IEntity
    {
        public TestChild()
        {
            Image = new BinaryFile(); //This is a complex type and must be initialized no matter what
        }
        
        public string Name { get; set; }
        public BinaryFile Image { get; set; }
        public virtual TestPerson ParentPerson { get; set; }

        public int Id { get; set; }
        public void Add()
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
        public void Delete()
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
        virtual protected void AddInternal()
        {
            CreateRepo().ExecuteMethod("Add", this);
        }
        virtual protected void DeleteInternal()
        {
            CreateRepo().ExecuteMethod("Delete", this);
        }
        protected object CreateRepo()
        {
            return RepoFactory.CreateForRuntimeType(GetDbSetType());
        }
        private Type GetDbSetType()
        {
            var dataSetType = GetType();
            // ReSharper disable PossibleNullReferenceException
            if (dataSetType.FullName.StartsWith("System.Data.Entity.DynamicProxies.")) dataSetType = dataSetType.BaseType;
            // ReSharper restore PossibleNullReferenceException
            return dataSetType;
        }

        virtual public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }

        public IEntity ConstructVirtualProperties()
        {
            return this;
        }
    }
}
