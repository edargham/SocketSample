using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Common
{
    /// <summary>
    /// Utility class to serialize and deserialize objects into XML format.
    /// </summary>
    public class XMLSerializer
    {
        public static XDocument Serialize<T>(T instance)
        {
            return Serialize(typeof(T), instance);
        }

        public static XDocument Serialize(Type targetType, object instance)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(targetType);
                serializer.Serialize(stream, instance);

                stream.Flush();
                stream.Position = 0L;

                return XDocument.Load(stream);
            }
        }

        public static T Deserialize<T>(XDocument xml)
        {
            return (T)Deserialize(typeof(T), xml);
        }

        public static object Deserialize(Type targetType, XDocument xml)
        {
            StringReader reader = new StringReader(xml.ToString());

            object serializer = new XmlSerializer(targetType).Deserialize(reader);

            return serializer;
        }
    }
}
