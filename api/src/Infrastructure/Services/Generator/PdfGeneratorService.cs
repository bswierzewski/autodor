using Application.Interfaces;
using Infrastructure.Interfaces;
using iText.Html2pdf;

namespace Infrastructure.Services.Generator;

public class PDFGeneratorService(ICustomPropertiesProvider customPropertiesProvider) : IPDFGeneratorService
{
    public byte[] Generate(string htmlContent)
    {
        using var memoryStream = new MemoryStream();

        HtmlConverter.ConvertToPdf(htmlContent, memoryStream, customPropertiesProvider.ConverterProperties);

        return memoryStream.ToArray();
    }
}
