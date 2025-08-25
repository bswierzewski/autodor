using System.Xml.Serialization;

namespace Autodor.Modules.Products.Infrastructure.Helpers;

public static class XmlHelper
{
    public static T DeserializeXml<T>(this string xml)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xml);
        return (T)serializer.Deserialize(reader)!;
    }
}