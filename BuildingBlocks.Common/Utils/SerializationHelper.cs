using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BuildingBlocks.Common.Utils
{
    public static class SerializationHelper
    {
        public static string Serialize(this object value)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            var serializer = new XmlSerializer(value.GetType());
            var output = new StringBuilder();
            var xmlWriter = XmlWriter.Create(output);
            serializer.Serialize(xmlWriter, value);
            xmlWriter.Close();
            return output.ToString();
        }

        public static T DeserializeTo<T>(this string xml)
        {
            return (T) DeserializeTo(xml, typeof (T));
        }

        public static object DeserializeTo(this string xml, Type type)
        {
            if (type == null) 
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException("xml");

            var serivalizer = new XmlSerializer(type);
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                var deserializedObject = serivalizer.Deserialize(stringReader);
                return deserializedObject;
            }
            finally
            {
                if (stringReader != null)
                    stringReader.Close();
            }
        }
    }
}