using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Contractors.Commands.CreateContractor;

public class CreateContractorCommand : IRequest
{
    public string Name { get; set; }
    public string City { get; set; }
    public string NIP { get; set; }
    public string ZipCode { get; set; }
    public string Street { get; set; }
    public string Email { get; set; }
}

public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand>
{
    private readonly ILogger<CreateContractorCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateContractorCommandHandler(ILogger<CreateContractorCommandHandler> logger,
        IApplicationDbContext context,
        IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    public async Task Handle(CreateContractorCommand request, CancellationToken cancellationToken)
    {
        var contractor = _mapper.Map<Contractor>(request);

        await _context.Contractors.AddAsync(contractor, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
