using System.Xml;
using System.Xml.Serialization;

namespace Infrastructure.Extensions
{
    public static class XmlExtensions
    {
        public static T Deserialize<T>(this XmlNode node)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (XmlReader reader = new XmlNodeReader(node))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
