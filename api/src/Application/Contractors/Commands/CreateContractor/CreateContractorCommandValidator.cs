namespace Application.Contractors.Commands.CreateContractor;

public class CreateContractorCommandValidator : AbstractValidator<CreateContractorCommand>
{
    public CreateContractorCommandValidator()
    {
        RuleFor(c => c.Name).NotNull().NotEmpty();
        RuleFor(c => c.City).NotNull().NotEmpty();
        RuleFor(c => c.NIP).NotNull().NotEmpty();
        RuleFor(c => c.ZipCode).NotNull().NotEmpty();
        RuleFor(c => c.Street).NotNull().NotEmpty();
    }
}
