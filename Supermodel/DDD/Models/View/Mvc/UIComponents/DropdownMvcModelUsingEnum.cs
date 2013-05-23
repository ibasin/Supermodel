using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ReflectionMapper;
using Supermodel.DDD.Models.Extensions;

namespace Supermodel.DDD.Models.View.Mvc.UIComponents
{
    public class DropdownMvcModelUsingEnum<EnumT> : DropdownMvcModel, ICustomMapper where EnumT : struct, IConvertible
    {
        public class EnumOption : Option
        {
            public EnumOption(EnumT value) : base(value.ToString(CultureInfo.InvariantCulture), value.GetDescription(), value.IsDisabled()) { }
            public EnumT EnumValue
            {
                get
                {
                    return (EnumT)Enum.Parse(typeof(EnumT), Value);
                }
            }
        }

        public DropdownMvcModelUsingEnum()
        {
            var enumValues = new List<object>();
            foreach (var item in Enum.GetValues(typeof(EnumT))) enumValues.Add(item);
            enumValues = enumValues.OrderBy(x => x.GetScreenOrder()).ToList();

            foreach (var option in enumValues) Options.Add(new EnumOption((EnumT)option));
            SelectedValue = enumValues[0].ToString();
        }

        public DropdownMvcModelUsingEnum(EnumT selectedEnum) : this()
        {
            SelectedEnum = selectedEnum;
        }

        public EnumT? SelectedEnum
        {
            get
            {
                if (string.IsNullOrEmpty(SelectedValue)) return null;
                return (EnumT)Enum.Parse(typeof(EnumT), SelectedValue);
            }
            set
            {
                if (value == null) SelectedValue = "";
                else SelectedValue = ((EnumT)value).ToString(CultureInfo.InvariantCulture);
            }
        }

        public virtual object MapFromObjectCustom(object obj, Type objType)
        {
            if (objType != typeof(EnumT) && objType != typeof(EnumT?)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));
            SelectedEnum = (EnumT?)obj;
            return this;
        }
        // ReSharper disable RedundantAssignment
        public virtual object MapToObjectCustom(object obj, Type objType)
        {
            if (objType != typeof(EnumT) && objType != typeof(EnumT?)) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1}", GetType().Name, objType.Name));
            if (objType == typeof(EnumT) && SelectedEnum == null) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1} because {0} is null but {1} is not nullable", GetType().Name, objType.Name));
            obj = SelectedEnum;
            return obj;
        }
        // ReSharper restore RedundantAssignment
    }
}
