using Dactra.DTOs.Sponsorship;
using Dactra.DTOs;

namespace Dactra.Services.Implementation
{
    public class PatientReferralService : IPatientReferralService
    {
        private readonly IPatientReferralRepository _referralRepo;
        private readonly ISponsorshipRepository _sponsorshipRepo;
        private readonly ApplicationDbContext _context;

        public PatientReferralService(
            IPatientReferralRepository referralRepo,
            ISponsorshipRepository sponsorshipRepo,
            ApplicationDbContext context)
        {
            _referralRepo = referralRepo;
            _sponsorshipRepo = sponsorshipRepo;
            _context = context;
        }


        public async Task<PatientReferralResponseDTO> ReferPatientAsync(int doctorId, ReferPatientDTO dto)
        {
            var sponsorship = await _sponsorshipRepo.GetByIdAsync(dto.SponsorshipId)
                ?? throw new KeyNotFoundException("Sponsorship not found");

            if (sponsorship.DoctorId != doctorId)
                throw new UnauthorizedAccessException("This sponsorship does not belong to you");

            if (sponsorship.Status != SponsorshipStatus.Active)
                throw new InvalidOperationException("Sponsorship is not active");

            var inCare = await _context.PatientDoctorCares
                .AnyAsync(x => x.DoctorId == doctorId
                             && x.PatientId == dto.PatientId
                             && x.IsActive
                             && x.ExpiresAtUtc > DateTime.UtcNow);

            if (!inCare)
                throw new InvalidOperationException("Patient is not under your care");

            var providerId = sponsorship.MedicalTestProviderId;

            var validOfferingIds = await _context.ProviderOfferings
                .Where(o => o.ProviderId == providerId
                         && dto.ProviderOfferingIds.Contains(o.Id))
                .Select(o => o.Id)
                .ToListAsync();

            if (validOfferingIds.Count != dto.ProviderOfferingIds.Distinct().Count())
                throw new InvalidOperationException(
                    "One or more selected services do not belong to this provider");

            var referral = new PatientReferral
            {
                PatientId = dto.PatientId,
                DoctorId = doctorId,
                SponsorshipId = dto.SponsorshipId,
                Status = ReferralStatus.Pending,
                ReferralServices = validOfferingIds.Select(id => new PatientReferralItem
                {
                    ProviderOfferingId = id
                }).ToList()
            };

            var created = await _referralRepo.CreateAsync(referral);

            var full = await _referralRepo.GetByIdAsync(created.Id)
                ?? throw new Exception("Failed to load referral after creation");

            return Map(full);
        }


        public async Task<PatientReferralResponseDTO> MarkAsReceivedAsync(int referralId, int providerId)
        {
            var referral = await _referralRepo.GetByIdAsync(referralId)
                ?? throw new KeyNotFoundException("Referral not found");

            if (referral.Sponsorship.MedicalTestProviderId != providerId)
                throw new UnauthorizedAccessException("This referral does not belong to your account");

            if (referral.Status == ReferralStatus.Received)
                throw new InvalidOperationException("Already marked as received");

            referral.Status = ReferralStatus.Received;
            referral.ReceivedAtUtc = DateTime.UtcNow;

            await _referralRepo.UpdateAsync(referral);
            return Map(referral);
        }

        public async Task<IEnumerable<PatientReferralResponseDTO>> GetReferralsByProviderAsync(int providerId)
        {
            var referrals = await _referralRepo.GetReferralsByProviderAsync(providerId);
            return referrals.Select(Map);
        }

        public async Task<IEnumerable<PatientReferralResponseDTO>> GetReferralsByDoctorAsync(int doctorId)
        {
            var referrals = await _referralRepo.GetReferralsByDoctorAsync(doctorId);
            return referrals.Select(Map);
        }

        public async Task<ProviderReferralsDashboardDTO> GetProviderReferralsDashboardAsync(int providerId)
        {
            var referrals = await _referralRepo.GetReferralsByProviderAsync(providerId);
            var list = referrals.ToList();

            var items = list.Select(r =>
            {
                var discount = r.Sponsorship?.DiscountPercentage ?? 0;

                var services = r.ReferralServices?.Select(s =>
                {
                    var price = s.ProviderOffering?.Price ?? 0;
                    var discountAmount = Math.Round(price * discount / 100, 2);
                    var priceAfter = Math.Round(price - discountAmount, 2);

                    return new ReferralServiceItemDTO
                    {
                        ProviderOfferingId = s.ProviderOfferingId,
                        ServiceName = s.ProviderOffering?.TestService?.Name ?? string.Empty,
                        ServiceDescription = s.ProviderOffering?.TestService?.Description ?? string.Empty,
                        PriceBeforeDiscount = price,
                        DiscountAmount = discountAmount,
                        PriceAfterDiscount = priceAfter,
                        Duration = s.ProviderOffering?.Duration ?? TimeSpan.Zero
                    };
                }).ToList() ?? new();

                var totalBefore = services.Sum(x => x.PriceBeforeDiscount);
                var totalAfter = services.Sum(x => x.PriceAfterDiscount);

                return new ProviderReferralItemDTO
                {
                    ReferralId = r.Id,
                    PatientId = r.PatientId,
                    PatientName = $"{r.Patient?.FirstName} {r.Patient?.LastName}".Trim(),
                    PhoneNumber = r.Patient?.User?.PhoneNumber ?? string.Empty,
                    DoctorId = r.DoctorId,
                    DoctorName = $"{r.Doctor?.FirstName} {r.Doctor?.LastName}".Trim(),
                    DoctorSpeciality = r.Doctor?.specialization?.Name ?? string.Empty,
                    Status = r.Status,
                    ReferredAtUtc = r.ReferredAtUtc,
                    Services = services,
                    TotalPriceBeforeDiscount = totalBefore,
                    TotalPriceAfterDiscount = totalAfter,
                    TotalSaved = Math.Round(totalBefore - totalAfter, 2)
                };
            }).ToList();

            return new ProviderReferralsDashboardDTO
            {
                Total = items.Count,
                Pending = items.Count(x => x.Status == ReferralStatus.Pending),
                Received = items.Count(x => x.Status == ReferralStatus.Received),
                Referrals = items
            };
        }

        public async Task<PagedResultDto<DoctorCarePatientItemDTO>> GetDoctorCarePatientsAsync(int doctorId,PaginationDto pagination,string? searchTerm = null)
        {
            var pagedRecords = await _referralRepo.GetActiveCarePatientsByDoctorPagedAsync(doctorId, pagination, searchTerm);
            var mappedItems = pagedRecords.Items.Select(x => new DoctorCarePatientItemDTO
            {
                PatientId = x.PatientId,
                PatientName = $"{x.Patient?.FirstName} {x.Patient?.LastName}".Trim(),
                PhoneNumber = x.Patient?.User?.PhoneNumber ?? string.Empty,
                Email = x.Patient?.User?.Email ?? string.Empty
            }).ToList();
            return new PagedResultDto<DoctorCarePatientItemDTO>
            {
                Items = mappedItems,
                TotalCount = pagedRecords.TotalCount,
                Page = pagedRecords.Page,
                PageSize = pagedRecords.PageSize
            };
        }

        public async Task<PagedReferralResponseDto> GetProviderReferralsPagedAsync(
            int providerId,
            int page = 1,
            int pageSize = 10,
            ReferralStatus? status = null)
        {
            var pagination = new PaginationDto
            {
                Page = page,
                PageSize = pageSize
            };

            var pagedResult = await _referralRepo
                .GetReferralsByProviderPagedAsync(providerId, pagination, status);

            var items = pagedResult.Items.Select(r =>
            {
                var discount = r.Sponsorship?.DiscountPercentage ?? 0;

                var services = r.ReferralServices?.Select(s =>
                {
                    var price = s.ProviderOffering?.Price ?? 0;
                    var discountAmount = Math.Round(price * discount / 100, 2);
                    var priceAfter = Math.Round(price - discountAmount, 2);

                    return new ReferralServiceItemDTO
                    {
                        ProviderOfferingId = s.ProviderOfferingId,
                        ServiceName = s.ProviderOffering?.TestService?.Name ?? string.Empty,
                        ServiceDescription = s.ProviderOffering?.TestService?.Description ?? string.Empty,
                        PriceBeforeDiscount = price,
                        DiscountAmount = discountAmount,
                        PriceAfterDiscount = priceAfter,
                        Duration = s.ProviderOffering?.Duration ?? TimeSpan.Zero
                    };
                }).ToList() ?? new();

                var totalBefore = services.Sum(x => x.PriceBeforeDiscount);
                var totalAfter = services.Sum(x => x.PriceAfterDiscount);

                return new PatientReferralResponseDTO
                {
                    Id = r.Id,
                    PatientId = r.PatientId,
                    PatientName = $"{r.Patient?.FirstName} {r.Patient?.LastName}".Trim(),
                    PatientPhone = r.Patient?.User?.PhoneNumber ?? string.Empty,
                    PatientEmail = r.Patient?.User?.Email ?? string.Empty,
                    DoctorId = r.DoctorId,
                    DoctorName = $"{r.Doctor?.FirstName} {r.Doctor?.LastName}".Trim(),
                    SponsorshipId = r.SponsorshipId,
                    DiscountPercentage = discount,
                    Status = r.Status,
                    ReferredAtUtc = r.ReferredAtUtc,
                    ReceivedAtUtc = r.ReceivedAtUtc,
                    Services = services,
                    TotalBeforeDiscount = totalBefore,
                    TotalAfterDiscount = totalAfter,
                    TotalSaved = Math.Round(totalBefore - totalAfter, 2)
                };
            }).ToList();

            return new PagedReferralResponseDto
            {
                Items = items,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }

        private PatientReferralResponseDTO Map(PatientReferral r)
        {
            var discount = r.Sponsorship?.DiscountPercentage ?? 0;

            var serviceItems = r.ReferralServices?.Select(s =>
            {
                var price = s.ProviderOffering?.Price ?? 0;
                var discountAmount = Math.Round(price * discount / 100, 2);
                var priceAfter = Math.Round(price - discountAmount, 2);

                return new ReferralServiceItemDTO
                {
                    ProviderOfferingId = s.ProviderOfferingId,
                    ServiceName = s.ProviderOffering?.TestService?.Name ?? string.Empty,
                    ServiceDescription = s.ProviderOffering?.TestService?.Description ?? string.Empty,
                    PriceBeforeDiscount = price,
                    DiscountAmount = discountAmount,
                    PriceAfterDiscount = priceAfter,
                    Duration = s.ProviderOffering?.Duration ?? TimeSpan.Zero
                };
            }).ToList() ?? new();

            var totalBefore = serviceItems.Sum(x => x.PriceBeforeDiscount);
            var totalAfter = serviceItems.Sum(x => x.PriceAfterDiscount);

            return new PatientReferralResponseDTO
            {
                Id = r.Id,
                PatientId = r.PatientId,
                PatientName = $"{r.Patient?.FirstName} {r.Patient?.LastName}".Trim(),
                PatientPhone = r.Patient?.User?.PhoneNumber ?? string.Empty,
                PatientEmail = r.Patient?.User?.Email ?? string.Empty,
                DoctorId = r.DoctorId,
                DoctorName = $"{r.Doctor?.FirstName} {r.Doctor?.LastName}".Trim(),
                SponsorshipId = r.SponsorshipId,
                DiscountPercentage = discount,
                Status = r.Status,
                ReferredAtUtc = r.ReferredAtUtc,
                ReceivedAtUtc = r.ReceivedAtUtc,
                Services = serviceItems,
                TotalBeforeDiscount = totalBefore,
                TotalAfterDiscount = totalAfter,
                TotalSaved = Math.Round(totalBefore - totalAfter, 2)
            };
        }
    }
}
