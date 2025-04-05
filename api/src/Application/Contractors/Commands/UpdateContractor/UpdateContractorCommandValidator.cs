namespace Application.Contractors.Commands.UpdateContractor;

class UpdateContractorCommandValidator : AbstractValidator<UpdateContractorCommand>
{
    public UpdateContractorCommandValidator()
    {
        RuleFor(c => c.Name).NotNull().NotEmpty();
        RuleFor(c => c.City).NotNull().NotEmpty();
        RuleFor(c => c.NIP).NotNull().NotEmpty();
        RuleFor(c => c.ZipCode).NotNull().NotEmpty();
        RuleFor(c => c.Street).NotNull().NotEmpty();
    }
}
