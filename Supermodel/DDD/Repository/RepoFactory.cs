using System;
using System.Collections.Generic;
using Supermodel.DDD.Models.Domain;
using ReflectionMapper;

namespace Supermodel.DDD.Repository
{
    public static class RepoFactory
    {
        public static ISqlDataRepo<EntityT> Create<EntityT>() where EntityT : class, IEntity, new()
        {
            foreach (var customFactory in _customRepoFactoryList)
            {
                var repo = customFactory.CreateRepo<EntityT>();
                if (repo != null) return repo;
            }
            return new SqlLinqEFSimpleRepo<EntityT>();
        }

        public static object CreateForRuntimeType(Type entityType)
        {
            return ReflectionHelper.ExecuteStaticGenericMethod(typeof(RepoFactory), "Create", new[] { entityType });
        }

        public static void RegisterCustomRepoFactory(IRepoFactory customRepoFactory)
        {
            _customRepoFactoryList.Add(customRepoFactory);
        }
        // ReSharper disable InconsistentNaming
        private static readonly List<IRepoFactory> _customRepoFactoryList = new List<IRepoFactory>();
        // ReSharper restore InconsistentNaming
    }
}
