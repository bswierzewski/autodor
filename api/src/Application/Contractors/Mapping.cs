using Application.Contractors.Commands.CreateContractor;
using Application.Contractors.Commands.UpdateContractor;
using AutoMapper;
using Domain.Entities;

namespace Application.Contractors;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<CreateContractorCommand, Contractor>();

        CreateMap<UpdateContractorCommand, Contractor>();
    }
}
