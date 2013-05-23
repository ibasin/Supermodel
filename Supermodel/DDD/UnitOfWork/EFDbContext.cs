using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Reflection;
using ReflectionMapper;
using Supermodel.DDD.Models.CommonValueTypes;
using Supermodel.DDD.Models.Data;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.UnitOfWork
{
    public abstract class EFDbContext : DbContext
    {
        #region Constructors
        protected EFDbContext(string databaseNameOrConnectionString) : base(databaseNameOrConnectionString)
        {
            CommitOnDispose = true;
            IsReadOnly = false;
        }
        #endregion

        #region Properties
        public bool CommitOnDispose { get; set; }
        public bool IsReadOnly { get; protected set; }
        #endregion

        #region Methods
        public EFDbContext MakeReadOnly()
        {
            IsReadOnly = true;
            return this;
        }
        
        //Returns the assemblies containing entities and entity config classes for this unit of work. Default returns AppDomain.CurrentDomain.GetAssemblies(). Override it for finer control 
        protected virtual Assembly[] GetDomainEntitiesAssemblies() { return AppDomain.CurrentDomain.GetAssemblies(); }

        //This is just in case somebody calls SaveChanges() directly
        public override int SaveChanges()
        {
            try
            {
                return IsReadOnly ? 0 : base.SaveChanges();
            }
            // ReSharper disable RedundantCatchClause
            #pragma warning disable 168
            catch (Exception ex)
            {
                throw;
            }
            #pragma warning restore 168
            // ReSharper restore RedundantCatchClause
        }

        public new void Dispose()
        {
            if (CommitOnDispose && !IsReadOnly) SaveChanges();
            base.Dispose();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            IsReadOnly = false; //If we are creating a new model, we must set ReadOnly to false even it was created as readOnly. Otherwise our Seed will not get saved
            
            base.OnModelCreating(modelBuilder);

            //Set up custom supermodel datatypes
            modelBuilder.Configurations.Add(new BinaryFileTypeMapping());//.Add(new EmailTypeMapping());//.Add(new EFEnumTypeMapping())

            //Set up configurations that are derived from EFDataModelConfigForEntity and EFDataModelConfigForValueType
            foreach (var assembly in GetDomainEntitiesAssemblies())
            {
                Type[] assemblyTypes;
                try { assemblyTypes = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException) { continue; }
                
                foreach (var assemblyType in assemblyTypes)
                {
                    if (assemblyType.IsAbstract || assemblyType.IsGenericType) continue;

                    var baseTypeOfEFDataModelConfigForEntity = ReflectionHelper.IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(assemblyType, typeof(EFDataModelConfigForEntity<>));
                    var baseTypeOfEFDataModelConfigForValueType = ReflectionHelper.IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(assemblyType, typeof(EFDataModelConfigForValueType<>));

                    if (baseTypeOfEFDataModelConfigForEntity != null)
                    {
                        var instanceOfEFDataModelConfigForEntityInstance = ReflectionHelper.CreateType(assemblyType);
                        //modelBuilder.Configurations.ExecuteGenericMethod("Add", baseTypeOfEFDataModelConfigForEntity.GetGenericArguments(), instanceOfEFDataModelConfigForEntityInstance);
                        this.ExecuteNonPublicGenericMethod("AddToConfiguration", baseTypeOfEFDataModelConfigForEntity.GetGenericArguments(), modelBuilder, instanceOfEFDataModelConfigForEntityInstance);
                    }
                    else if (baseTypeOfEFDataModelConfigForValueType != null)
                    {
                        var instanceOfEFDataModelConfigForValueType = ReflectionHelper.CreateType(assemblyType);
                        //modelBuilder.Configurations.ExecuteGenericMethod("Add", baseTypeOfEFDataModelConfigForValueType.GetGenericArguments(), instanceOfEFDataModelConfigForValueType);
                        this.ExecuteNonPublicGenericMethod("AddToConfiguration", baseTypeOfEFDataModelConfigForValueType.GetGenericArguments(), modelBuilder, instanceOfEFDataModelConfigForValueType);
                    }
                }
            }

            //Set up configurations that implememnt IEntity
            foreach (var assembly in GetDomainEntitiesAssemblies())
            {
                Type[] assemblyTypes;
                try { assemblyTypes = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException) { continue; }

                foreach (var assemblyType in assemblyTypes)
                {
                    if (assemblyType.IsAbstract) continue;
                    if (typeof(IEntity).IsAssignableFrom(assemblyType)) modelBuilder.ExecuteGenericMethod("Entity", new[] { assemblyType });
                }
            }
        }

        // We call this method through reflection to avoid dealing with Configurations.Add() overloads
        protected ConfigurationRegistrar AddToConfiguration<EntityT>(DbModelBuilder modelBuilder, EntityTypeConfiguration<EntityT> entityTypeConfiguration) where EntityT : class
        {
            return modelBuilder.Configurations.Add(entityTypeConfiguration);
        }
        #endregion
    }
}
