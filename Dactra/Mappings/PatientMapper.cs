namespace Dactra.Mappings
{
    public class PatientMapper : Profile
    {
        public PatientMapper()
        {
            CreateMap<PatientCompleteDTO, PatientProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForAllMembers(opt =>
                {
                    opt.Condition((src, dest, srcMember) => srcMember != null);
                });

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
                .ForMember(dest => dest.address,
                    opt => opt.MapFrom(src => src.Address != null
                        ? $"{src.Address.Name}"
                        : null))
                .ForMember(dest => dest.AddressId,
                    opt => opt.MapFrom(src => src.AddressId));

            CreateMap<PatientUpdateDTO, PatientProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForAllMembers(opt =>
                {
                    opt.Condition((src, dest, srcMember) =>
                    {
                        if (srcMember == null) return false;

                        if (srcMember is string s)
                            return !string.IsNullOrWhiteSpace(s);

                        return true;
                    });
                });
        }
    }
}
