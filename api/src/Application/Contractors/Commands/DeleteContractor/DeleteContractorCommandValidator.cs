namespace Application.Contractors.Commands.DeleteContractor;

public class DeleteContractorCommandValidator : AbstractValidator<DeleteContractorCommand>
{
    public DeleteContractorCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("Musisz wskazać kontrahenta");
    }
}
