using Dactra.Migrations;

namespace Dactra.Mappings
{
    public class MedicalTestsProviderMapper : Profile
    {
        public MedicalTestsProviderMapper()
        {
            CreateMap<MedicalTestProviderDTO, MedicalTestProviderProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.approvalStatus, opt => opt.Ignore())
                .ForMember(dest => dest.WorkingHours, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<MedicalTestsProviderUpdateDTO, MedicalTestProviderProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.approvalStatus, opt => opt.Ignore())
                    .ForMember(dest => dest.approvalStatus, opt => opt.Ignore()).ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src =>
                    src.WorkingHours.Select(w => new LabsWorkingHour
                    {
                        Day = w.Day,
                        From = w.From,
                        To = w.To
                    }).ToList()))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<MedicalTestProviderProfile, MedicalTestsProviderResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LicenceNo, opt => opt.MapFrom(src => src.LicenceNo))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.About, opt => opt.MapFrom(src => src.About))
                .ForMember(dest => dest.Avg_Rating, opt => opt.MapFrom(src => src.Avg_Rating))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null))
                .ForMember(dest => dest.profileImageUrl,
                    opt => opt.MapFrom(src => src.User != null && !string.IsNullOrEmpty(src.User.ImageUrl) ? src.User.ImageUrl : null));

            CreateMap<LabsWorkingHour, WorkingHourDTO>();

            CreateMap<MedicalTestProviderProfile, MedicalTestProviderSearchResultDTO>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Avg_Rating))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.User != null ? src.User.ImageUrl : null));
        }
    }
}
