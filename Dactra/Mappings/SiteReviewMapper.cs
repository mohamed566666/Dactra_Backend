namespace Dactra.Mappings
{
    public class SiteReviewMapper : Profile
    {
        public SiteReviewMapper()
        {
            CreateMap<SiteReviewRequestDto, SiteReview>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Reviewer, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerName, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsVisible, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<SiteReviewRequestDto, SiteReview>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerUserId, opt => opt.Ignore())
                .ForMember(dest => dest.Reviewer, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerName, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewerImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsVisible, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SiteReview, SiteReviewResponse>()
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.ReviewerName))
                .ForMember(dest => dest.ReviewerImageUrl, opt => opt.MapFrom(src => src.ReviewerImageUrl));
        }
    }
}
