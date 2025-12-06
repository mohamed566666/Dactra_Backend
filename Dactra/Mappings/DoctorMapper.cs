using AutoMapper;
using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
using Dactra.Models;
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
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));

            CreateMap<DoctorUpdateDTO, DoctorProfile>()
                .ForMember(dest => dest.specialization, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt =>
                {
                    opt.Condition((src, dest, srcMember) => srcMember != null);
                });
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
