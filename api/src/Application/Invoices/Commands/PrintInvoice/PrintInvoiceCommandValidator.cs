namespace Application.Invoices.Commands.PrintInvoice;
public class CreateInvoiceCommandValidator : AbstractValidator<PrintInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(o => o.OrderId)
        .NotEmpty();
    }
}