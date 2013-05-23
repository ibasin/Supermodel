using System.ComponentModel.DataAnnotations;

namespace Supermodel.DDD.Models.Domain
{
    public interface IEntity : IValidatableObject
    {
        int Id { get; }
        IEntity ConstructVirtualProperties();
        void Add();
        void Delete();
    }
}