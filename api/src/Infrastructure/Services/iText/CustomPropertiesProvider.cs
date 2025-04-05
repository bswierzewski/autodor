using Infrastructure.Common;
using Infrastructure.Interfaces;
using iText.Html2pdf;
using iText.Layout.Font;

namespace Infrastructure.Services.iText
{
    public class CustomPropertiesProvider : ICustomPropertiesProvider
    {
        public ConverterProperties ConverterProperties { get; set; }

        private static readonly string namespaceFont = "Infrastructure.Resources.Fonts";
        private string arial = $"{namespaceFont}.Arial.ttf";
        private string arialbd = $"{namespaceFont}.Arialbd.ttf";
        private string arialbi = $"{namespaceFont}.Arialbi.ttf";
        private string ariali = $"{namespaceFont}.Ariali.ttf";

        public CustomPropertiesProvider()
        {
            var fontSet = new FontSet();

            fontSet.AddFont(ResourcesConverter.ConvertResourceToBytes(arial));
            fontSet.AddFont(ResourcesConverter.ConvertResourceToBytes(arialbd));
            fontSet.AddFont(ResourcesConverter.ConvertResourceToBytes(arialbi));
            fontSet.AddFont(ResourcesConverter.ConvertResourceToBytes(ariali));

            var fontProvider = new FontProvider(fontSet, "Arial");

            var converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);

            ConverterProperties = converterProperties;
        }
    }
}
