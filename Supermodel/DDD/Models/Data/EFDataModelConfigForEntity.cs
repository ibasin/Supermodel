using System.Data.Entity.ModelConfiguration;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.Data
{
    public abstract class EFDataModelConfigForEntity<EntityT> : EntityTypeConfiguration<EntityT> where EntityT : class, IEntity, new() {}
}
