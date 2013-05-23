using System;
using System.Linq.Expressions;

using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Repository 
{
    public interface ISqlDataRepo<EntityT> : ISqlDataRepoGenericTypeIgnorant, ISqlDataRepoReadOnly<EntityT>, ISqlDataRepoWriteOnly<EntityT> where EntityT : class, IEntity, new()
    {
        EntityT GetOrCreate(Expression<Func<EntityT, bool>> predicatesLambda);
    }
}
