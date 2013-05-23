using System.ComponentModel.DataAnnotations;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;

namespace Supermodel.DDD.Models.View
{
    // ReSharper disable UnusedTypeParameter
    interface IViewModelForEntity<EntityT> : IValidatableObject, ICustomMapper where EntityT : class, IEntity, new()
    // ReSharper restore UnusedTypeParameter
    {
        int Id { get; set; }
    }
}
