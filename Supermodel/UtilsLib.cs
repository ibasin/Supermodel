using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Supermodel
{
    public static class UtilsLib
    {
        public static Dictionary<string, object> ObjectToCaseSensitiveDictionary(object values)
        {
            var dict = new Dictionary<string, object>(StringComparer.Ordinal);
            if (values != null)
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(values))
                {
                    object val = prop.GetValue(values);
                    dict[prop.Name] = val;
                }
            }
            return dict;
        }

        public static string GenerateAttributesString(IDictionary<string, object> dict)
        {
            if (dict == null) return " ";
            var sb = new StringBuilder();
            foreach (var pair in dict) sb.AppendFormat("{0}='{1}'", pair.Key, pair.Value);
            return " " + sb.ToString().Trim() + " ";
        }
    }
}
