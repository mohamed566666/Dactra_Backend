using Dactra.DTOs.RatingDTOs;

namespace Dactra.Mappings
{
    public class RatingMapper : Profile
    {
        public RatingMapper()
        {
            CreateMap<Rating, RatingResponseDTO>()
                .ForMember(dest => dest.RatingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment ?? string.Empty))
                .ForMember(dest => dest.RatedAt, opt => opt.MapFrom(src => src.Rated_At))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src =>
                    src.Patient != null
                        ? $"{src.Patient.FirstName} {src.Patient.LastName}"
                        : string.Empty));

            CreateMap<IEnumerable<Rating>, List<RatingResponseDTO>>()
                .ConvertUsing((src, dest, context) => src.Select(r => context.Mapper.Map<RatingResponseDTO>(r)).ToList());
        }
    }
}
