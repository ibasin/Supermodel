using System.Data.Entity;
using TestDomain.Entities;

namespace TestDomain.Supermodel
{
    public static class DataInfoInitializer
    {
        public static void Seed(TestDbContext context)
        {
            context.Set<TestProfession>().Add(new TestProfession { Profession = "Teacher"});
            context.Set<TestProfession>().Add(new TestProfession { Profession = "Doctor" });
            context.Set<TestProfession>().Add(new TestProfession { Profession = "Laweyr" });
            context.Set<TestProfession>().Add(new TestProfession { Profession = "Software Developer" });

            context.Set<TestGroup>().Add(new TestGroup { Name = "Group 1" });
            context.Set<TestGroup>().Add(new TestGroup { Name = "Group 2" });
            context.Set<TestGroup>().Add(new TestGroup { Name = "Group 3" });
            context.Set<TestGroup>().Add(new TestGroup { Name = "Group 4" });

            //context.SaveChanges();
        }
    }

    public class TestDropCreateDatabaseIfModelChanges : DropCreateDatabaseIfModelChanges<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            base.Seed(context);
            DataInfoInitializer.Seed(context);
        }
    }

    public class TestDropCreateDatabaseAlways : DropCreateDatabaseAlways<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            base.Seed(context);
            DataInfoInitializer.Seed(context);
        }
    }

    public class TestCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<TestDbContext>
    {
        protected override void Seed(TestDbContext context)
        {
            base.Seed(context);
            DataInfoInitializer.Seed(context);
        }
    }
}
