namespace Dactra.Mappings
{
    public class DoctorMapper : Profile
    {
        public DoctorMapper()
        {
            CreateMap<DoctorCompleteDTO, DoctorProfile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.specialization, opt => opt.Ignore())
                .ForMember(dest => dest.SpecializationId,
                    opt => opt.MapFrom(src => src.MajorId))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<DoctorProfile, DoctorProfileResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? string.Empty))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? string.Empty))
                .ForMember(dest => dest.About, opt => opt.MapFrom(src => src.About ?? string.Empty))
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => src.Address != null ? src.Address : string.Empty))
                .ForMember(dest => dest.age,
                    opt => opt.MapFrom(src =>
                        src.DateOfBirth != default(DateTime) ? CalculateYears(src.DateOfBirth) : 0))
                .ForMember(dest => dest.YearsOfExperience,
                    opt => opt.MapFrom(src =>
                        src.StartingCareerDate != default(DateTime) ? CalculateYears(src.StartingCareerDate) : 0))
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.Avg_Rating == default(decimal) ? 0m : src.Avg_Rating))
                .ForMember(dest => dest.SpecializationName,
                    opt => opt.MapFrom(src => src.specialization != null ? src.specialization.Name : string.Empty))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.profileImageUrl,
                    opt => opt.MapFrom(src => src.User != null && !string.IsNullOrEmpty(src.User.ImageUrl) ? src.User.ImageUrl : null));

            CreateMap<DoctorProfile, DoctorsFilterResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.specialization != null ? src.specialization.Name : "N/A"))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Avg_Rating))
                .ForMember(dest => dest.profileImageUrl,
                    opt => opt.MapFrom(src => src.User != null && !string.IsNullOrEmpty(src.User.ImageUrl) ? src.User.ImageUrl : null));

            CreateMap<(IEnumerable<DoctorProfile> doctors, int totalCount, DoctorFilterDTO filter), PaginatedDoctorsResponseDTO>()
                .ForMember(dest => dest.Doctors, opt => opt.MapFrom(src => src.doctors))
                .ForMember(dest => dest.CurrentPage, opt => opt.MapFrom(src => src.filter.PageNumber))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.filter.PageSize))
                .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.totalCount))
                .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => (int)Math.Ceiling(src.totalCount / (double)src.filter.PageSize)));

            CreateMap<DoctorProfile, DoctorsResponseDTO>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.FirstName ?? string.Empty))

            .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src => src.LastName ?? string.Empty))

            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))

            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty))

            .ForMember(dest => dest.gender,
                opt => opt.MapFrom(src => src.Gender))

            .ForMember(dest => dest.SpecializationName,
                opt => opt.MapFrom(src => src.specialization != null ? src.specialization.Name : string.Empty))

            .ForMember(dest => dest.age,
                opt => opt.MapFrom(src =>
                    src.DateOfBirth != default
                        ? CalculateYears(src.DateOfBirth)
                        : 0))

            .ForMember(dest => dest.YearsOfExperience,
                opt => opt.MapFrom(src =>
                    src.StartingCareerDate != default
                        ? CalculateYears(src.StartingCareerDate)
                        : 0))

            .ForMember(dest => dest.AverageRating,
                opt => opt.MapFrom(src => src.Avg_Rating))

            .ForMember(dest => dest.About,
                opt => opt.MapFrom(src => src.About ?? string.Empty))

            .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => src.Address ?? string.Empty))
            .ForMember(dest => dest.Qualifications,
                opt => opt.Ignore())
            .ForMember(dest => dest.ratings,
                opt => opt.Ignore())
            .ForMember(dest => dest.profileImageUrl,
                opt => opt.MapFrom(src => src.User != null && !string.IsNullOrEmpty(src.User.ImageUrl) ? src.User.ImageUrl : null));

        }
        private static int CalculateYears(DateTime date)
        {
            var today = DateTime.Today;
            var years = today.Year - date.Year;
            if (date.Date > today.AddYears(-years))
                years--;
            return Math.Max(years, 0);
        }
    }
}
