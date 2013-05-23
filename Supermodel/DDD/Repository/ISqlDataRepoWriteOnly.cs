using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Repository
{
    public interface ISqlDataRepoWriteOnly<in EntityT> where EntityT : class, IEntity, new()
    {
        void Add(EntityT item);
        void Delete(EntityT item);
    }
}