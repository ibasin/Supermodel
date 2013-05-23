using System.Globalization;
using ReflectionMapper;
using Supermodel.DDD.Repository;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class DropdownMvcModelUsing<MvcModelT> : DropdownMvcModelForEntity where MvcModelT : MvcModelForEntityCore
    {
        public DropdownMvcModelUsing()
        {
            var mvcModelForEntitybaseType = ReflectionHelper.IfClassADerivedFromClassBGetFullGenericBaseTypeOfB(typeof(MvcModelT), typeof(MvcModelForEntity<>));
            if (mvcModelForEntitybaseType == null) throw new SupermodelException("DropdownMvcModelUsing<MvcModelT> has invalid type parameter");
            var entityType = mvcModelForEntitybaseType.GetGenericArguments()[0];
            
            Options = ((ISqlDataRepoGenericTypeIgnorant)RepoFactory.CreateForRuntimeType(entityType)).GetDropdownOptions<MvcModelT>();
            SelectedValue = "";
        }

        public DropdownMvcModelUsing(int selectedId): this()
        {
            SelectedValue = selectedId.ToString(CultureInfo.InvariantCulture);
        }
    }
}