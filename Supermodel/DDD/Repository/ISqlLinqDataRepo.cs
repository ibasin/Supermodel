using System.Linq;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Repository
{
    public interface ISqlLinqDataRepo<EntityT> : ISqlDataRepo<EntityT> where EntityT : class, IEntity, new()
    {
        IQueryable<EntityT> Items { get; }
    }
}