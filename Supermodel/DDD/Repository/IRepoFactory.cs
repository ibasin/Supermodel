using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Repository 
{
	public interface IRepoFactory
	{
	    ISqlDataRepo<EntityT> CreateRepo<EntityT>() where EntityT : class, IEntity, new();
	}
}
