namespace Application.Invoices.Commands.CreateInvoice;
public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(o => o.Orders)
        .NotEmpty();
    }
}