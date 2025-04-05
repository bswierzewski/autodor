using Application.Invoices.Commands.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Invoices.Mappings
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Contractor, Kontrahent>()
                .ForMember(d => d.Nazwa, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Miejscowosc, o => o.MapFrom(s => s.City))
                .ForMember(d => d.Ulica, o => o.MapFrom(s => s.Street))
                .ForMember(d => d.KodPocztowy, o => o.MapFrom(s => s.ZipCode))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                ;
        }
    }
}
