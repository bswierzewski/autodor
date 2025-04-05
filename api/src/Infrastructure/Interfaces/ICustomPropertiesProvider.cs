using iText.Html2pdf;

namespace Infrastructure.Interfaces
{
    public interface ICustomPropertiesProvider
    {
        ConverterProperties ConverterProperties { get; set; }
    }
}
