namespace Dactra.Mappings
{
    public class MajorMapper : Profile
    {
        public MajorMapper()
        {
            CreateMap<MajorBasicsDTO, Majors>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore());

            CreateMap<Majors, MajorsResponseDTO>()
                .ForMember(dest => dest.IconPath, opt => opt.MapFrom(src => src.Iconpath));

            CreateMap<MajorBasicsDTO, Majors>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
