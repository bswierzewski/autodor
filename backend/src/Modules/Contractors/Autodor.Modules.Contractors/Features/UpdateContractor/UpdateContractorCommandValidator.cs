using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Features.UpdateContractor;

public class UpdateContractorRequestValidator : AbstractValidator<UpdateContractorRequest>
{
    public UpdateContractorRequestValidator(ContractorsDbContext dbContext)
    {
        RuleFor(x => x.Command!.NIP)
            .NotEmpty().WithMessage("NIP jest wymagany.")
            .Length(10).WithMessage("NIP musi zawierać dokładnie 10 cyfr.")
            .Matches(@"^\d{10}$").WithMessage("NIP może zawierać wyłącznie cyfry.")
            .MustAsync(async (request, nip, ct) => !await dbContext.Contractors.AnyAsync(c => c.NIP == new TaxId(nip) && c.Id != new ContractorId(request.Id), ct))
            .WithMessage("Kontrahent z podanym NIP już istnieje.");

        RuleFor(x => x.Command!.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana.")
            .MaximumLength(200).WithMessage("Nazwa może mieć maksymalnie 200 znaków.");

        RuleFor(x => x.Command!.Street)
            .NotEmpty().WithMessage("Ulica jest wymagana.")
            .MaximumLength(200).WithMessage("Ulica może mieć maksymalnie 200 znaków.");

        RuleFor(x => x.Command!.City)
            .NotEmpty().WithMessage("Miasto jest wymagane.")
            .MaximumLength(100).WithMessage("Miasto może mieć maksymalnie 100 znaków.");

        RuleFor(x => x.Command!.ZipCode)
            .NotEmpty().WithMessage("Kod pocztowy jest wymagany.")
            .MaximumLength(20).WithMessage("Kod pocztowy może mieć maksymalnie 20 znaków.");

        RuleFor(x => x.Command!.Email)
            .NotEmpty().WithMessage("Adres e-mail jest wymagany.")
            .EmailAddress().WithMessage("Adres e-mail jest nieprawidłowy.")
            .MaximumLength(100).WithMessage("Adres e-mail może mieć maksymalnie 100 znaków.");
    }
}
