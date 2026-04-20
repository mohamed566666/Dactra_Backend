using Dactra.DTOs.Sponsorship;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class SponsorshipService : ISponsorshipService
    {
        private readonly ISponsorshipRepository _sponsorshipRepo;
        private readonly IDoctorProfileRepository _doctorRepo;
        private readonly IPatientReferralRepository _referralRepo;

        public SponsorshipService(
            ISponsorshipRepository sponsorshipRepo,
            IDoctorProfileRepository doctorRepo,
            IPatientReferralRepository referralRepo)
        {
            _sponsorshipRepo = sponsorshipRepo;
            _doctorRepo = doctorRepo;
            _referralRepo = referralRepo;
        }

        // ─── Provider sends offer ──────────────────────────────────

        public async Task<SponsorshipResponseDTO> SendOfferAsync(
            int providerId,
            MedicalTestProviderType providerType,
            SendOfferDTO dto)
        {
            var doctor = await _doctorRepo.GetByIdAsync(dto.DoctorId)
                ?? throw new KeyNotFoundException("Doctor not found");

            var hasActive = await _sponsorshipRepo
                .DoctorHasActiveSponsorAsync(dto.DoctorId, providerType);

            if (hasActive)
                throw new InvalidOperationException(
                    $"Doctor already has an active {providerType} sponsor");

            var entity = new DoctorMedicalTestSponsor
            {
                DoctorId = dto.DoctorId,
                MedicalTestProviderId = providerId,
                ProviderType = providerType,
                OfferContent = dto.OfferContent,
                DiscountPercentage = dto.DiscountPercentage,
                Status = SponsorshipStatus.Pending,
                IsCounterOffer = false
            };

            var created = await _sponsorshipRepo.CreateAsync(entity);
            var full = await _sponsorshipRepo.GetByIdAsync(created.Id)
                ?? throw new Exception("Failed to reload sponsorship");
            return MapToResponse(full);
        }

        // ─── Doctor: Accept ────────────────────────────────────────

        public async Task<SponsorshipResponseDTO> AcceptOfferAsync(int sponsorshipId, int doctorId)
        {
            var offer = await _sponsorshipRepo.GetByIdAsync(sponsorshipId)
                ?? throw new KeyNotFoundException("Offer not found");

            if (offer.DoctorId != doctorId)
                throw new UnauthorizedAccessException();

            if (offer.Status != SponsorshipStatus.Pending)
                throw new InvalidOperationException("Offer is not pending");

            var hasActive = await _sponsorshipRepo
                .DoctorHasActiveSponsorAsync(doctorId, offer.ProviderType);

            if (hasActive)
                throw new InvalidOperationException(
                    $"You already have an active {offer.ProviderType} sponsor");

            offer.Status = SponsorshipStatus.Active;
            offer.RespondedAtUtc = DateTime.UtcNow;

            await _sponsorshipRepo.UpdateAsync(offer);
            return MapToResponse(offer);
        }

        // ─── Doctor: Reject ────────────────────────────────────────

        public async Task<SponsorshipResponseDTO> RejectOfferAsync(int sponsorshipId, int doctorId)
        {
            var offer = await _sponsorshipRepo.GetByIdAsync(sponsorshipId)
                ?? throw new KeyNotFoundException("Offer not found");

            if (offer.DoctorId != doctorId)
                throw new UnauthorizedAccessException();

            if (offer.Status != SponsorshipStatus.Pending)
                throw new InvalidOperationException("Offer is not pending");

            offer.Status = SponsorshipStatus.Rejected;
            offer.RespondedAtUtc = DateTime.UtcNow;

            await _sponsorshipRepo.UpdateAsync(offer);
            return MapToResponse(offer);
        }

        // ─── Doctor: Counter ───────────────────────────────────────

        public async Task<SponsorshipResponseDTO> CounterOfferAsync(
            int sponsorshipId,
            int doctorId,
            CounterOfferDTO dto)
        {
            var original = await _sponsorshipRepo.GetByIdAsync(sponsorshipId)
                ?? throw new KeyNotFoundException("Offer not found");

            if (original.DoctorId != doctorId)
                throw new UnauthorizedAccessException();

            if (original.Status != SponsorshipStatus.Pending)
                throw new InvalidOperationException("Offer is not pending");

            original.Status = SponsorshipStatus.Cancelled;
            original.RespondedAtUtc = DateTime.UtcNow;
            await _sponsorshipRepo.UpdateAsync(original);

            var counter = new DoctorMedicalTestSponsor
            {
                DoctorId = doctorId,
                MedicalTestProviderId = original.MedicalTestProviderId,
                ProviderType = original.ProviderType,
                OfferContent = original.OfferContent,
                DiscountPercentage = dto.NewDiscountPercentage,
                Status = SponsorshipStatus.Pending,
                IsCounterOffer = true,
                ParentOfferId = original.Id
            };

            var created = await _sponsorshipRepo.CreateAsync(counter);
            var full = await _sponsorshipRepo.GetByIdAsync(created.Id)
                ?? throw new Exception("Failed to reload counter offer");
            return MapToResponse(full);
        }

        // ─── Provider: Accept counter ──────────────────────────────

        public async Task<SponsorshipResponseDTO> AcceptCounterAsync(int sponsorshipId, int providerId)
        {
            var counter = await _sponsorshipRepo.GetByIdAsync(sponsorshipId)
                ?? throw new KeyNotFoundException("Counter offer not found");

            if (counter.MedicalTestProviderId != providerId)
                throw new UnauthorizedAccessException();

            if (!counter.IsCounterOffer || counter.Status != SponsorshipStatus.Pending)
                throw new InvalidOperationException("Invalid state");

            var hasActive = await _sponsorshipRepo
                .DoctorHasActiveSponsorAsync(counter.DoctorId, counter.ProviderType);

            if (hasActive)
                throw new InvalidOperationException(
                    $"Doctor already has an active {counter.ProviderType} sponsor");

            counter.Status = SponsorshipStatus.Active;
            counter.RespondedAtUtc = DateTime.UtcNow;

            await _sponsorshipRepo.UpdateAsync(counter);
            return MapToResponse(counter);
        }

        // ─── Provider: Reject counter ──────────────────────────────

        public async Task<SponsorshipResponseDTO> RejectCounterAsync(int sponsorshipId, int providerId)
        {
            var counter = await _sponsorshipRepo.GetByIdAsync(sponsorshipId)
                ?? throw new KeyNotFoundException("Counter offer not found");

            if (counter.MedicalTestProviderId != providerId)
                throw new UnauthorizedAccessException();

            if (!counter.IsCounterOffer || counter.Status != SponsorshipStatus.Pending)
                throw new InvalidOperationException("Invalid state");

            counter.Status = SponsorshipStatus.Rejected;
            counter.RespondedAtUtc = DateTime.UtcNow;

            await _sponsorshipRepo.UpdateAsync(counter);
            return MapToResponse(counter);
        }

        // ─── Queries (Legacy) ────────────────────────────────────

        public async Task<IEnumerable<SponsorshipResponseDTO>> GetProviderOffersAsync(int providerId)
        {
            var offers = await _sponsorshipRepo.GetProviderOffersAsync(providerId);
            return offers.Select(MapToResponse);
        }

        public async Task<IEnumerable<SponsorshipResponseDTO>> GetActiveSponsorsForProviderAsync(int providerId)
        {
            var offers = await _sponsorshipRepo.GetActiveSponsorsForProviderAsync(providerId);
            return offers.Select(MapToResponse);
        }

        public async Task<IEnumerable<SponsorshipResponseDTO>> GetDoctorOffersAsync(int doctorId)
        {
            var offers = await _sponsorshipRepo.GetDoctorOffersAsync(doctorId);
            return offers.Select(MapToResponse);
        }

        public async Task<SponsorshipResponseDTO?> GetActiveSponsorshipAsync(
            int doctorId,
            MedicalTestProviderType type)
        {
            var offer = await _sponsorshipRepo.GetActiveSponsorshipAsync(doctorId, type);
            return offer is null ? null : MapToResponse(offer);
        }

        // ─── Dashboard (Legacy) ─────────────────────────────────

        public async Task<ProviderOffersSummaryDTO> GetProviderOffersSummaryAsync(int providerId)
        {
            var (pending, counter, rejected) =
                await _sponsorshipRepo.GetProviderOfferCountsAsync(providerId);

            return new ProviderOffersSummaryDTO
            {
                PendingCount = pending,
                CounterCount = counter,
                RejectedCount = rejected
            };
        }

        public async Task<IEnumerable<ProviderOfferItemDTO>> GetProviderOffersByStatusAsync(
            int providerId,
            SponsorshipStatus status)
        {
            var offers = await _sponsorshipRepo
                .GetProviderOffersByStatusAsync(providerId, status);

            return offers.Select(x => MapToProviderOfferItem(x, status));
        }

        public async Task<ActiveSponsorsOverviewDTO> GetActiveSponsorsOverviewAsync(int providerId)
        {
            var activeSponsors = await _sponsorshipRepo
                .GetActiveSponsorsForProviderAsync(providerId);

            var patientCounts = await _sponsorshipRepo
                .GetPatientsSentCountPerDoctorAsync(providerId);

            var items = activeSponsors.Select(s =>
            {
                patientCounts.TryGetValue(s.DoctorId, out var count);
                return new ActiveSponsorItemDTO
                {
                    DoctorId = s.DoctorId,
                    DoctorName = s.Doctor is not null
                        ? $"{s.Doctor.FirstName} {s.Doctor.LastName}"
                        : string.Empty,
                    DoctorSpeciality = s.Doctor?.specialization?.Name ?? string.Empty,
                    DiscountPercentage = s.DiscountPercentage,
                    Description = s.OfferContent,
                    PatientsSentCount = count
                };
            }).ToList();

            return new ActiveSponsorsOverviewDTO
            {
                TotalDoctors = items.Count,
                TotalPatientsSent = items.Sum(x => x.PatientsSentCount),
                AverageDiscount = items.Count > 0
                    ? Math.Round(items.Average(x => x.DiscountPercentage), 2)
                    : 0,
                Doctors = items
            };
        }

        public async Task<SponsorshipResponseDTO> CancelActiveSponsorshipAsync(int sponsorshipId, int actorId, string actorRole)
        {
            var sponsorship = await _sponsorshipRepo.GetByIdAsync(sponsorshipId)
                ?? throw new KeyNotFoundException("Sponsorship not found");

            if (sponsorship.Status != SponsorshipStatus.Active)
                throw new InvalidOperationException("Only active sponsorships can be cancelled");

            var isDoctor = actorRole == "Doctor" && sponsorship.DoctorId == actorId;
            var isProvider = actorRole == "MedicalTestProvider" && sponsorship.MedicalTestProviderId == actorId;

            if (!isDoctor && !isProvider)
                throw new UnauthorizedAccessException("You are not part of this sponsorship");

            sponsorship.Status = SponsorshipStatus.Cancelled;
            sponsorship.RespondedAtUtc = DateTime.UtcNow;

            await _sponsorshipRepo.UpdateAsync(sponsorship);
            return MapToResponse(sponsorship);
        }

        // =============== PAGINATED METHODS ===============

        public async Task<PagedSponsorshipResponseDto> GetProviderOffersPagedAsync(
            int providerId,
            int page = 1,
            int pageSize = 10)
        {
            var pagination = new PaginationDto { Page = page, PageSize = pageSize };
            var pagedResult = await _sponsorshipRepo.GetProviderOffersPagedAsync(providerId, pagination);

            return new PagedSponsorshipResponseDto
            {
                Items = pagedResult.Items.Select(MapToResponse).ToList(),
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }

        public async Task<PagedSponsorshipResponseDto> GetDoctorOffersPagedAsync(
            int doctorId,
            int page = 1,
            int pageSize = 10)
        {
            var pagination = new PaginationDto { Page = page, PageSize = pageSize };
            var pagedResult = await _sponsorshipRepo.GetDoctorOffersPagedAsync(doctorId, pagination);

            return new PagedSponsorshipResponseDto
            {
                Items = pagedResult.Items.Select(MapToResponse).ToList(),
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }

        public async Task<PagedSponsorshipResponseDto> GetProviderOffersByStatusPagedAsync(
            int providerId,
            SponsorshipStatus? status,
            int page = 1,
            int pageSize = 10)
        {
            var pagination = new StatusPaginationDto
            {
                Page = page,
                PageSize = pageSize,
                Status = status
            };

            var pagedResult = await _sponsorshipRepo
                .GetProviderOffersByStatusPagedAsync(providerId, pagination);

            var targetStatus = status ?? SponsorshipStatus.Pending;

            return new PagedSponsorshipResponseDto
            {
                Items = pagedResult.Items.Select(x => MapToResponse(x)).ToList(),
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }

        public async Task<PagedActiveSponsorsOverviewDTO> GetActiveSponsorsOverviewPagedAsync(
            int providerId,
            int page = 1,
            int pageSize = 10)
        {
            var pagination = new PaginationDto { Page = page, PageSize = pageSize };
            var pagedResult = await _sponsorshipRepo
                .GetActiveSponsorsForProviderPagedAsync(providerId, pagination);

            // Get ALL patient counts for summary statistics (not just current page)
            var allActiveSponsors = await _sponsorshipRepo
                .GetActiveSponsorsForProviderAsync(providerId);

            var allPatientCounts = await _sponsorshipRepo
                .GetPatientsSentCountPerDoctorAsync(providerId);

            var totalDoctorsCount = allActiveSponsors.Count();

            var totalPatientsSentSum = allActiveSponsors.Sum(s =>
            {
                allPatientCounts.TryGetValue(s.DoctorId, out var count);
                return count;
            });

            var averageDiscountValue = totalDoctorsCount > 0
                ? Math.Round(allActiveSponsors.Average(x => x.DiscountPercentage), 2)
                : 0;

            var items = pagedResult.Items.Select(s =>
            {
                allPatientCounts.TryGetValue(s.DoctorId, out var patientCount);
                return new ActiveSponsorItemDTO
                {
                    SponsorshipId = s.Id,
                    DoctorId = s.DoctorId,
                    DoctorName = s.Doctor is not null
                        ? $"{s.Doctor.FirstName} {s.Doctor.LastName}"
                        : string.Empty,
                    DoctorSpeciality = s.Doctor?.specialization?.Name ?? string.Empty,
                    DiscountPercentage = s.DiscountPercentage,
                    Description = s.OfferContent,
                    PatientsSentCount = patientCount
                };
            }).ToList();

            return new PagedActiveSponsorsOverviewDTO
            {
                Items = items,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage,
                TotalDoctors = totalDoctorsCount,
                TotalPatientsSent = totalPatientsSentSum,
                AverageDiscount = (double) averageDiscountValue
            };
        }

        public async Task<PagedSponsorshipResponseDto> GetProviderOffersByFilterPagedAsync(int providerId,OfferFilterStatus filterStatus,int page = 1,int pageSize = 10)
        {
            var pagination = new PaginationDto { Page = page, PageSize = pageSize };

            var pagedResult = await _sponsorshipRepo
                .GetProviderOffersByFilterPagedAsync(providerId, filterStatus, pagination);

            return new PagedSponsorshipResponseDto
            {
                Items = pagedResult.Items.Select(MapToResponse).ToList(),
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }

        // ─── Mapping ───────────────────────────────────────────────

        private SponsorshipResponseDTO MapToResponse(DoctorMedicalTestSponsor x) => new()
        {
            Id = x.Id,
            DoctorId = x.DoctorId,
            DoctorName = x.Doctor is not null
                ? $"{x.Doctor.FirstName} {x.Doctor.LastName}"
                : string.Empty,
            MedicalTestProviderId = x.MedicalTestProviderId,
            ProviderName = x.MedicalTestProvider?.Name ?? string.Empty,
            ProviderType = x.ProviderType,
            OfferContent = x.OfferContent,
            DiscountPercentage = x.DiscountPercentage,
            Status = x.Status,
            IsCounterOffer = x.IsCounterOffer,
            ParentOfferId = x.ParentOfferId,
            RequestedAtUtc = x.RequestedAtUtc,
            RespondedAtUtc = x.RespondedAtUtc,
            CounterOffers = x.CounterOffers?.Select(MapToResponse).ToList() ?? new(),
            DoctorSpeciality = x.Doctor?.specialization?.Name ?? string.Empty
        };

        private ProviderOfferItemDTO MapToProviderOfferItem(DoctorMedicalTestSponsor x, SponsorshipStatus requestedStatus)
        {
            OriginalOfferSnapshotDTO? originalSnapshot = null;

            if (requestedStatus == SponsorshipStatus.Cancelled && x.ParentOffer is not null)
            {
                originalSnapshot = new OriginalOfferSnapshotDTO
                {
                    Id = x.ParentOffer.Id,
                    DiscountPercentage = x.ParentOffer.DiscountPercentage,
                    Description = x.ParentOffer.OfferContent,
                    OfferDate = x.ParentOffer.RequestedAtUtc
                };
            }

            return new ProviderOfferItemDTO
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                DoctorName = x.Doctor is not null
                    ? $"{x.Doctor.FirstName} {x.Doctor.LastName}"
                    : string.Empty,
                DoctorSpeciality = x.Doctor?.specialization?.Name ?? string.Empty,
                DiscountPercentage = x.DiscountPercentage,
                Description = x.OfferContent,
                OfferDate = x.RequestedAtUtc,
                Status = requestedStatus,
                OriginalOffer = originalSnapshot
            };
        }
    }
}