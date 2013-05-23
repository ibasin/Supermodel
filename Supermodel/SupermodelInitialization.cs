using System.Web.Mvc;
using Supermodel.DDD.Models.View.Mvc.Metadata;
using Supermodel.DDD.Repository;
using Supermodel.Mvc.DefaultModelBinders;

namespace Supermodel
{
    public static class SupermodelInitialization
    {
        public static void Init(IRepoFactory customRepoFactory = null, ModelMetadataProvider customModelMetadataProvider = null, IModelBinder defaultModelBinder = null)
        {
            if (customModelMetadataProvider == null) ModelMetadataProviders.Current = new SupermodelTemplateMetadataProvider();
            else ModelMetadataProviders.Current = customModelMetadataProvider;

            if (defaultModelBinder == null) ModelBinders.Binders.DefaultBinder = new SupermodelDefaultModelBinder();
            else ModelBinders.Binders.DefaultBinder = defaultModelBinder;

            if (customRepoFactory != null) RepoFactory.RegisterCustomRepoFactory(customRepoFactory);
        }
    }
}
