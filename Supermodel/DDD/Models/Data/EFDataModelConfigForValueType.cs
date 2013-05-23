using System.Data.Entity.ModelConfiguration;

namespace Supermodel.DDD.Models.Data
{
    public abstract class EFDataModelConfigForValueType<T> : ComplexTypeConfiguration<T> where T:class {}
}
