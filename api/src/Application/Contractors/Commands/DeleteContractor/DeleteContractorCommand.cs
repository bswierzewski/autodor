using Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Contractors.Commands.DeleteContractor;

public record DeleteContractorCommand(int Id) : IRequest;

public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand>
{
    private readonly ILogger<DeleteContractorCommandHandler> _logger;
    private readonly IApplicationDbContext _context;

    public DeleteContractorCommandHandler(ILogger<DeleteContractorCommandHandler> logger,
        IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contractor = await _context.Contractors.FindAsync([request.Id], cancellationToken);

            if (contractor == null)
                return;

            _context.Contractors.Remove(contractor);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            throw;
        }
    }
}