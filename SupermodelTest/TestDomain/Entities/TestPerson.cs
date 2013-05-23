using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ReflectionMapper;
using Supermodel.DDD.Models.Domain;
using Supermodel.DDD.Models.View.Mvc;
using Supermodel.DDD.Models.View.Mvc.JQMobile;
using Supermodel.DDD.Models.View.Mvc.Metadata;
using Supermodel.DDD.Models.View.Mvc.UIComponents;

namespace TestDomain.Entities
{
    public class TestPersonMobileMvcModel : JQMobile.MvcModelForEntity<TestPerson>
    {
        public TestPersonMobileMvcModel()
        {
            Name = new JQMobile.TextBoxForStringMvcModel { Placeholder = "Enter Name Here" };
            NotHookedUpSearch = new JQMobile.SearchBoxMvcModel { Placeholder = "Not Hooked Up Search Here" };
            PIN = new JQMobile.TextBoxForPasswordMvcModel();
            ImageView = new JQMobile.ImageMvcModel { HtmlAttributesAsObj = new { style = "max-width: 100%; max-height: 200px;" } };
        }

        [NotRMapped, HideLabel] public JQMobile.SearchBoxMvcModel NotHookedUpSearch { get; set; }
        [Required] public JQMobile.TextBoxForStringMvcModel Name { get; set; }
        
        [RMapTo(PropertyName = "Image")] public JQMobile.ImageMvcModel ImageView { get; set; }
        [RMapTo(PropertyName = "Image")] public JQMobile.BinaryFileMvcModel ImageFile { get; set; }

        public JQMobile.SliderMvcModel ApprovalRating { get; set; }
        [DisplayName("Date of Birth")] public JQMobile.DateMvcModel DOB { get; set; }
        public JQMobile.TextBoxForDoubleMvcModel Height { get; set; }
        public JQMobile.TextBoxForIntMvcModel FamilySize { get; set; }
        public JQMobile.TextBoxForTelephoneMvcModel Phone { get; set; }
        public JQMobile.TextBoxForEmailMvcModel Email { get; set; }
        public JQMobile.TextBoxForUrlMvcModel Url { get; set; }
        [NotRMapped, DisplayName("Secret PIN")] public JQMobile.TextBoxForPasswordMvcModel PIN { get; set; }
        public JQMobile.DropdownMvcModelUsing<TestProfesisonMvcModel> Profession { get; set; }
        //[Required] public Mobile.RadioSelectVerticalMvcModelUsing<TestProfesisonMvcModel> Profession { get; set; }
        //[Required] public Mobile.RadioSelectHorizontalMvcModelUsing<TestProfesisonMvcModel> Profession { get; set; }
        public JQMobile.DropdownMvcModelUsingEnum<TestPerson.SexEnum> Sex { get; set; }
        public JQMobile.RadioSelectVerticalMvcModelUsingEnum<TestPerson.RaceEnum> Race { get; set; }
        public JQMobile.RadioSelectHorizontalMvcModelUsingEnum<TestPerson.PoliticalAffiliationEnum> PoliticalAffiliation { get; set; } 
        public JQMobile.ToggleSwitchMvcModel RegisteredToVote { get; set; }
        public JQMobile.Checkbox SecurityClearance { get; set; }
        public JQMobile.CheckboxesListVerticalMvcModel<TestGroupMvcModel> Groups { get; set; }
        //public JQMobile.CheckboxesListHorizontalMvcModel<TestGroupMvcModel> Groups { get; set; } 

        public ICollection<TestChildMobileMvcModel> Children { get; set; }
        
        public override string Label { get { return Name.Value; } }
    }
    
    public class TestPersonMvcModel : MvcModelForEntity<TestPerson>
    {
        [Required] public string Name { get; set; }
        public DateMvcModel DOB { get; set; }
        ///*[Required]*/ public DropdownMvcModelUsingEnum<TestPerson.SexEnum> Sex { get; set; }
        //public RadioSelectFormModelUsingEnum<TestPerson.RaceEnum> Race { get; set; }
        ///*[Required]*/ public DropdownMvcModelUsing<TestProfesisonMvcModel> Profession { get; set; } 
        ///*[Required]*/ public CheckboxesListMvcModelUsing<TestGroupMvcModel> Groups { get; set; }
        
        //public BinaryFileMvcModel Image { get; set; }

        public ICollection<TestChildMvcModel> Children { get; set; }

        public override string Label { get { return Name; } }
    }

    public class TestPerson : Entity
    {
        public enum RaceEnum { White, Black, Asian };
        public enum SexEnum { Man, Woman };
        public enum PoliticalAffiliationEnum { Democrat, Republican };

        [Required] public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public int? ApprovalRating { get; set; }
        public double? Height { get; set; }
        public int? FamilySize { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string PIN { get; set; }
        public SexEnum? Sex { get; set; }
        public RaceEnum? Race { get; set; }
        public PoliticalAffiliationEnum? PoliticalAffiliation { get; set; }

        public BinaryFile Image { get; set; }
        public bool? RegisteredToVote { get; set; }
        public bool? SecurityClearance { get; set; }
        
        public virtual TestProfession Profession { get; set; }

        public virtual ICollection<TestGroup> Groups { get; set; }
        public virtual ICollection<TestChild> Children { get; set; }

        protected override void DeleteInternal()
        {
            foreach (var child in Children.ToList()) child.Delete();
            base.DeleteInternal();
        }
    }
}
