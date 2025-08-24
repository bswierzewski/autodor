using Autodor.Modules.Invoicing.Application.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Autodor.Modules.Invoicing.Infrastructure.Services;

public class MockPdfGeneratorService : IPdfGeneratorService
{
    private readonly ILogger<MockPdfGeneratorService> _logger;

    public MockPdfGeneratorService(ILogger<MockPdfGeneratorService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(object invoiceData, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating mock PDF for invoice data");

        // Symulacja generowania PDF - w rzeczywistości użylibyśmy biblioteki jak iTextSharp czy DinkToPdf
        await Task.Delay(100, cancellationToken); // Symulacja czasu przetwarzania

        var mockPdfContent = GenerateMockPdfContent(invoiceData);
        var pdfBytes = Encoding.UTF8.GetBytes(mockPdfContent);

        _logger.LogInformation("Mock PDF generated, size: {Size} bytes", pdfBytes.Length);

        return pdfBytes;
    }

    private static string GenerateMockPdfContent(object invoiceData)
    {
        var invoiceDataStr = System.Text.Json.JsonSerializer.Serialize(invoiceData, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });

        return $@"%PDF-1.4
1 0 obj
<<
/Type /Catalog
/Pages 2 0 R
>>
endobj

2 0 obj
<<
/Type /Pages
/Kids [3 0 R]
/Count 1
>>
endobj

3 0 obj
<<
/Type /Page
/Parent 2 0 R
/MediaBox [0 0 612 792]
/Contents 4 0 R
>>
endobj

4 0 obj
<<
/Length {invoiceDataStr.Length + 100}
>>
stream
BT
/F1 12 Tf
50 750 Td
(MOCK INVOICE PDF - Generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}) Tj
0 -20 Td
(Invoice Data:) Tj
0 -15 Td
({invoiceDataStr.Replace("\n", " ").Replace("\r", " ")}) Tj
ET
endstream
endobj

xref
0 5
0000000000 65535 f 
0000000009 00000 n 
0000000058 00000 n 
0000000115 00000 n 
0000000209 00000 n 
trailer
<<
/Size 5
/Root 1 0 R
>>
startxref
{invoiceDataStr.Length + 400}
%%EOF";
    }
}