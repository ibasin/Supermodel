using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XMLParsingExtForLINQ
{
    public static class XmlParsingExtensions
    {
        public static XElement ValidateTag(this XElement element, string expectedName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting <{0}>", expectedName));
            if (element.Name != expectedName) throw new XmlParsingException(string.Format("XML parsing error: <{0}> tag while expecting <{1}>", element.Name, expectedName));
            return element;
        }

        public static XElement GetChildElement(this XElement element, string childElementName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting a child tag '{0}", childElementName));
            XElement childElement = element.Element(childElementName);
            if (childElement == null) throw new XmlParsingException(string.Format("XML parsing error: <{0}> tag does not contain a child tag <{1}>", element.Name, childElementName));
            return childElement;
        }

        public static IEnumerable<XElement> GetChildElements(this XElement element, string childElementName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting a child tag '{0}", childElementName));
            IEnumerable<XElement> childElements = element.Elements(childElementName);
            return childElements;
        }

        public static XElement SingleOrNull(this IEnumerable<XElement> elements)
        {
            switch (elements.Count())
            {
                case 0: return null;
                case 1: return elements.First();
                // ReSharper disable PossibleMultipleEnumeration
                default: throw new XmlParsingException(elements.Count() + " '" + elements.First().Name + "' elements while expecting only 1 element.");
                // ReSharper restore PossibleMultipleEnumeration
            }
        }

        public static bool AttributeExists(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null) return false;
            else return true;
        }

        public static bool ChildTagExists(this XElement element, string childElementName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting a child tag '{0}", childElementName));
            XElement childElement = element.Element(childElementName);
            if (childElement == null) return false;
            return true;
        }

        public static string GetAttributeValueStr(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null) return "";
            return attribute.Value;
        }

        public static string GetTagContentStr(this XElement element)
        {
            if (element == null) throw new XmlParsingException("XML parsing error: tag is NULL while expecting it to have value");
            return element.Value;
        }

        public static int? GetAttributeValueInt(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null || attribute.Value.Trim() == String.Empty) return null;
            try
            {
                return int.Parse(attribute.Value);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: attribute {0} value is '{1}' cannot be converted to integer'", attributeName, attribute.Value));
            }
        }

        public static long? GetAttributeValueLong(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null || attribute.Value.Trim() == String.Empty) return null;
            try
            {
                return long.Parse(attribute.Value);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: attribute {0} value is '{1}' cannot be converted to long'", attributeName, attribute.Value));
            }
        }

        public static DateTime? GetAttributeValueDate(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null || attribute.Value.Trim() == String.Empty) return null;
            try
            {
                string val = attribute.Value;
                if (attribute.Value.Contains('-') == false && attribute.Value.Length == 8)
                {
                    val = attribute.Value.Substring(0, 4) + "-" + attribute.Value.Substring(4, 2) + "-" + attribute.Value.Substring(6, 2);
                }
                return DateTime.Parse(val);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: attribute {0} value is '{1}' cannot be converted to DateTime'", attributeName, attribute.Value));
            }
        }

        public static decimal? GetAttributeValueDecimal(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null || attribute.Value.Trim() == String.Empty) return null;
            try
            {
                return decimal.Parse(attribute.Value.Replace("$", ""));
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: attribute {0} value is '{1}' cannot be converted to decimal'", attributeName, attribute.Value));
            }
        }

        private static bool ParseBoolYN(string value)
        {
            switch (value)
            {
                case "Y": return true;
                case "N": return false;
                default: throw new FormatException(string.Format("Unable to convert '{0}' into boolean (must be 'Y' or 'N')", value));
            }
        }

        public static bool? GetAttributeValueBool(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null || attribute.Value.Trim() == String.Empty) return null;
            try
            {
                return ParseBoolYN(attribute.Value);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: attribute {0} value is '{1}' cannot be converted to decimal'", attributeName, attribute.Value));
            }
        }

        public static int GetTagContentInt(this XElement element)
        {
            if (element == null) throw new XmlParsingException("XML parsing error: tag is NULL while expecting it to have value");
            try
            {
                return int.Parse(element.Value);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: tag value is '{0}' cannot be converted to integer'", element.Value));
            }
        }

        public static long GetTagContentLong(this XElement element)
        {
            if (element == null) throw new XmlParsingException("XML parsing error: tag is NULL while expecting it to have value");
            try
            {
                return long.Parse(element.Value);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: tag value is '{0}' cannot be converted to long'", element.Value));
            }
        }

        public static bool GetTagContentBool(this XElement element)
        {
            if (element == null) throw new XmlParsingException("XML parsing error: tag is NULL while expecting it to have value");
            try
            {
                return bool.Parse(element.Value);
            }
            catch (Exception)
            {
                throw new XmlParsingException(string.Format("XML parsing error: tag value is '{0}' cannot be converted to bool'", element.Value));
            }
        }

        public static XAttribute GetAttribute(this XElement element, string attributeName)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0}", attributeName));
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null) throw new XmlParsingException(string.Format("XML parsing error: tag {0} attribute {1} is NULL while expected", element.Name, attributeName));
            return attribute;
        }

        public static XElement ValidateAttributeValue(this XElement element, string attributeName, string expectedValue)
        {
            if (element == null) throw new XmlParsingException(string.Format("XML parsing error: tag is NULL while expecting it to have attribute {0} with value '{1}", attributeName, expectedValue));
            XAttribute attribute = element.GetAttribute(attributeName);
            if (attribute.Value != expectedValue) throw new XmlParsingException(string.Format("XML parsing error: attribute {0} value is '{1}' while expecting '{2}'", attributeName, attribute.Value, expectedValue));
            return element;
        }
    }

}
