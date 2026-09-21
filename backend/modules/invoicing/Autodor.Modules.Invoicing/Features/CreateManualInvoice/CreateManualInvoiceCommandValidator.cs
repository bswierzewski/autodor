using FluentValidation;

namespace Autodor.Modules.Invoicing.Features.CreateManualInvoice;

public sealed class CreateManualInvoiceCommandValidator : AbstractValidator<CreateManualInvoiceCommand>
{
    public CreateManualInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceNumber)
            .GreaterThan(0)
            .When(x => x.InvoiceNumber.HasValue)
            .WithMessage("Numer faktury musi być większy od zera.");

        RuleFor(x => x.ContractorNIP)
            .NotEmpty().WithMessage("NIP kontrahenta jest wymagany.")
            .Length(10).WithMessage("NIP musi zawierać dokładnie 10 cyfr.")
            .Matches(@"^\d{10}$").WithMessage("NIP może zawierać wyłącznie cyfry.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Faktura musi zawierać co najmniej jedną pozycję.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ItemNumber)
                .NotEmpty().WithMessage("Numer pozycji jest wymagany.")
                .MaximumLength(300).WithMessage("Numer pozycji może mieć maksymalnie 300 znaków.");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Ilość musi być większa od zera.");

            item.RuleFor(x => x.NetUnitPrice)
                .GreaterThan(0)
                .WithMessage("Cena netto musi być większa od zera.")
                .LessThan(100_000_000m)
                .WithMessage("Cena netto musi być mniejsza niż 100 000 000.");
        });
    }
}
