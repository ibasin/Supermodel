using System.Data.Entity;

using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Repository
{
    public interface ISqlLinqEFDataRepo<EntityT> : ISqlLinqDataRepo<EntityT> where EntityT : class, IEntity, new()
    {
        DbSet<EntityT> DbSet { get; }
    }
}