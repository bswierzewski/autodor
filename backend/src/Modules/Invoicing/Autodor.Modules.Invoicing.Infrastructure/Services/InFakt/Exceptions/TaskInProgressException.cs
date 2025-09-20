namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Exceptions;

public class TaskInProgressException : Exception
{
    public int ProcessingCode { get; }
    public string ProcessingDescription { get; }

    public TaskInProgressException(int processingCode, string processingDescription)
        : base($"Invoice task is still in progress. Code: {processingCode}, Description: {processingDescription}")
    {
        ProcessingCode = processingCode;
        ProcessingDescription = processingDescription;
    }
}