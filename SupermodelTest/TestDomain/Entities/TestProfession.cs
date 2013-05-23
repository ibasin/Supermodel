using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;

namespace TestDomain.Entities
{
    public class TestProfesisonMvcModel : MvcModelForEntity<TestProfession>
    {
        public string Profession { get; set; }

        public override string Label
        {
            get { return Profession; }
        }
    }
    
    public class TestProfession : Entity
    {
        public string Profession { get; set; }
    }
}
