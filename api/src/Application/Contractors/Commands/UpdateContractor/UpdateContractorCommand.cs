using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Contractors.Commands.UpdateContractor;

public class UpdateContractorCommand : IRequest<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string NIP { get; set; }
    public string ZipCode { get; set; }
    public string Street { get; set; }
    public string Email { get; set; }
}

public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand, int>
{
    private readonly ILogger<UpdateContractorCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateContractorCommandHandler(ILogger<UpdateContractorCommandHandler> logger, IApplicationDbContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = await _context.Contractors.FindAsync(request.Id, cancellationToken);

        if(contractor == null)
        {
            _logger.LogError($"Contractor with ID {request.Id} not found.");

            throw new NotFoundException();
        }

        _mapper.Map(request, contractor);

        await _context.SaveChangesAsync(cancellationToken);

        return contractor.Id;
    }
}
