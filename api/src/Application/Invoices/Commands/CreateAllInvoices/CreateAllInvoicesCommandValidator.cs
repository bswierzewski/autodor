namespace Application.Invoices.Commands.CreateAllInvoices;
public class CreateAllInvoicesCommandValidator : AbstractValidator<CreateAllInvoicesCommand>
{
    public CreateAllInvoicesCommandValidator()
    {
        RuleFor(o => o.DateFrom).NotEmpty();

        RuleFor(o => o.DateTo).NotEmpty();
    }
}