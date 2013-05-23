using System.Collections.Generic;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Repository
{
    public interface ISqlDataRepoReadOnly<out EntityT> where EntityT : class, IEntity, new()
    {
        EntityT GetById(int id);
        IEnumerable<EntityT> GetAll();
        EntityT GetSingle();
    }
}