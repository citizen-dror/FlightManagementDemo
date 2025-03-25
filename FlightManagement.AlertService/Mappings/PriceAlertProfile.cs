using AutoMapper;
using FlightManagement.Common.DTOs;
using FlightManagement.Domain.Entities;

namespace FlightManagement.AlertService.Mappings
{
    public class PriceAlertProfile : Profile
    {
        public PriceAlertProfile()
        {
            CreateMap<PriceAlertDto, PriceAlert>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<PriceAlert, PriceAlertDto>();
        }
    }
}
