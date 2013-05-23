using Supermodel.DDD.UnitOfWork;

namespace TestDomain.Supermodel
{
    public class TestDbContext : EFDbContext
    {
        public TestDbContext() : base("TestDb") { }
    }
}

