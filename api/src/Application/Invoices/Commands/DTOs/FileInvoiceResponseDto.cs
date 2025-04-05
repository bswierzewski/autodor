namespace Application.Invoices.Commands.DTOs;

public class FileInvoiceResponseDto
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
}
