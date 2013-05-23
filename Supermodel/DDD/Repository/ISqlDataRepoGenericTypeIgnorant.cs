using System.Collections.Generic;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.UIComponents;

namespace Supermodel.DDD.Repository
{
    public interface ISqlDataRepoGenericTypeIgnorant
    {
        IEntity GetIEntityById(int id);
        IEnumerable<IEntity> GetIEntityAll();
        List<DropdownMvcModel.Option> GetDropdownOptions<MvcModelT>() where MvcModelT : MvcModelForEntityCore;
        List<MultiSelectMvcModelCore.Option> GetMultiSelectOptions<MvcModelT>() where MvcModelT : MvcModelForEntityCore;
    }
}