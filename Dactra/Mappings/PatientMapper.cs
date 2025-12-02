using Dactra.DTOs.ProfilesDTOs.PatientDTOs;
using Dactra.Models;
using AutoMapper;

namespace Dactra.Mappings
{
    public class PatientMapper : Profile
    {
        public PatientMapper()
        {
            CreateMap<PatientProfile, PatientProfileResponseDTO>()
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(_ => "Patient"))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? string.Empty))
                .ForMember(dest => dest.Allergies, opt => opt.MapFrom(src => src.Allergies ?? string.Empty))
                .ForMember(dest => dest.ChronicDisease, opt => opt.MapFrom(src => src.ChronicDisease ?? string.Empty))
                ;
        }
    }
}
