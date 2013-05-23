using System.Collections.Generic;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;

namespace TestDomain.Entities
{
    public class TestGroupMvcModel : MvcModelForEntity<TestGroup>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //public BinaryFileFormModel Certificate { get; set; }
        public override string Label
        {
            get { return Name; }
        }

        public override bool IsDisabled
        {
            get
            {
                if (Name == "Group 1") return true;
                else return false;
            }
        }
    }

    public class TestGroup : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //public BinaryFile Certificate { get; set; }

        public virtual ICollection<TestPerson> People { get; set; }
    }
}
