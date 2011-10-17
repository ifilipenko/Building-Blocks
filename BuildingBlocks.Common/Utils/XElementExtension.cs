using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BuildingBlocks.Common.Utils
{
    public static class XElementExtension
    {
        public static string GetValueFromAttributeOrChildNodeValue(this XElement element, string attributeOrNode)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (string.IsNullOrEmpty(attributeOrNode))
                throw new ArgumentNullException("attributeOrNode");

            var attribute = element.Attribute(attributeOrNode) ??
                            element.Attributes()
                                .FirstOrDefault(a => a.Name.ToString().Equals(attributeOrNode, StringComparison.OrdinalIgnoreCase));
            if (attribute != null)
                return attribute.Value;

            var childElement = element.Element(attributeOrNode) ??
                               element.Elements()
                                    .FirstOrDefault(a => a.Name.ToString().Equals(attributeOrNode, StringComparison.OrdinalIgnoreCase));
            if (childElement == null)
                throw new InvalidOperationException("Attribute or child element is not contains in node \"" + element.Name + "\"");

            return childElement.Value;
        }

        public static string SaveToXml(this XElement element)
        {
            if (element == null) 
                throw new ArgumentNullException("element");

            using (var stringWriter = new StringWriter())
            {
                element.Save(stringWriter, SaveOptions.None);
                return stringWriter.ToString();
            }
        }

        public static int ToInt(this XAttribute attribute)
        {
            return attribute == null ? 0 : Convert.ToInt32(attribute.Value);
        }

        public static TValue RequaredAttribute<TValue>(this XElement element, string attibuteName)
        {
            if (element == null)
                throw new ArgumentException("Element is miss");
            XAttribute attribute = element.Attribute(attibuteName);
            if (attribute == null)
            {
                throw new ArgumentException(string.Format("Attribute \"{0}\" is missed in node {1}", attibuteName,
                                                          element));
            }

            if (string.IsNullOrEmpty(attribute.Value))
            {
                return default(TValue);
            }

            return (TValue) Convert.ChangeType(attribute.Value, typeof (TValue));
        }

        public static TValue OptionalAttribute<TValue>(this XElement element, string attibuteName)
        {
            if (element == null)
                return default(TValue);
            XAttribute attribute = element.Attribute(attibuteName);
            if (attribute == null)
                return default(TValue);

            if (string.IsNullOrEmpty(attribute.Value))
            {
                return default(TValue);
            }

            return (TValue) Convert.ChangeType(attribute.Value, typeof (TValue));
        }

        public static Type OptionalTypeAttribute(this XElement element, string attibuteName)
        {
            if (element == null)
                return null;
            XAttribute attribute = element.Attribute(attibuteName);
            if (attribute == null)
                return typeof(string);

            if (string.IsNullOrEmpty(attribute.Value))
            {
                return null;
            }

            try
            {
                return Type.GetType("System." + attribute.Value, true, true);
            }
            catch
            {
                return Type.GetType(attribute.Value, true, true);
            }
        }
    }
}