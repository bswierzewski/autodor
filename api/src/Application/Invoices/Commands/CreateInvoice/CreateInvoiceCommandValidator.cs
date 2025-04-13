namespace Application.Invoices.Commands.CreateInvoice;
public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(o => o.OrderIds).NotEmpty().NotNull();
        RuleFor(o => o.Dates).NotEmpty().NotNull();
        RuleFor(o => o.ContractorId).NotEmpty().NotNull();
    }
}