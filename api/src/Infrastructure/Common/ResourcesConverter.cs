using System.Reflection;

namespace Infrastructure.Common
{
    public static class ResourcesConverter
    {
        public static byte[] ConvertResourceToBytes(string resourcePath)
            => ConvertResource(resourcePath);        

        public static string ConvertResourceToBase64(string resourcePath)
            => Convert.ToBase64String(ConvertResourceToBytes(resourcePath));        

        private static byte[] ConvertResource(string resourcePath)
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)
                ?? throw new FileNotFoundException($"Resource not found: {resourcePath}");

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
