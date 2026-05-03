namespace Dactra.Repositories.Implementation
{
    public class SponsorshipRepository : ISponsorshipRepository
    {
        private readonly ApplicationDbContext _context;

        public SponsorshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorMedicalTestSponsor?> GetByIdAsync(int id)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.MedicalTestProvider)
                .Include(x => x.CounterOffers)
                .Include(x => x.ParentOffer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<DoctorMedicalTestSponsor>> GetProviderOffersAsync(int providerId)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.CounterOffers)
                .Include(x => x.ParentOffer)
                .Where(x => x.MedicalTestProviderId == providerId && !x.IsCounterOffer)
                .OrderByDescending(x => x.RequestedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorMedicalTestSponsor>> GetProviderOffersByStatusAsync(
            int providerId,
            OfferFilterStatus status)
        {
            if (status == OfferFilterStatus.Counter)
            {
                return await _context.DoctorMedicalTestSponsors
                    .Include(x => x.Doctor)
                        .ThenInclude(d => d.specialization)
                    .Include(x => x.ParentOffer)
                    .Include(x => x.CounterOffers)
                    .Where(x => x.MedicalTestProviderId == providerId
                             && x.IsCounterOffer
                             && x.Status == SponsorshipStatus.Pending)
                    .OrderByDescending(x => x.RequestedAtUtc)
                    .ToListAsync();
            }
            else if (status == OfferFilterStatus.Pending)
            {
                return await _context.DoctorMedicalTestSponsors
                    .Include(x => x.Doctor)
                        .ThenInclude(d => d.specialization)
                    .Include(x => x.ParentOffer)
                    .Include(x => x.CounterOffers)
                    .Where(x => x.MedicalTestProviderId == providerId
                             && !x.IsCounterOffer
                             && x.Status == SponsorshipStatus.Pending)
                    .OrderByDescending(x => x.RequestedAtUtc)
                    .ToListAsync();
            }
            else
            {
                return await _context.DoctorMedicalTestSponsors
                    .Include(x => x.Doctor)
                        .ThenInclude(d => d.specialization)
                    .Include(x => x.ParentOffer)
                    .Include(x => x.CounterOffers)
                    .Where(x => x.MedicalTestProviderId == providerId
                             && x.Status == SponsorshipStatus.Rejected)
                    .OrderByDescending(x => x.RequestedAtUtc)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderAsync(int providerId)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.ParentOffer)
                .Include(x => x.CounterOffers)
                .Where(x => x.MedicalTestProviderId == providerId
                         && x.Status == SponsorshipStatus.Active)
                .ToListAsync();
        }

        public async Task<DoctorMedicalTestSponsor> CreateAsync(DoctorMedicalTestSponsor entity)
        {
            await _context.DoctorMedicalTestSponsors.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(DoctorMedicalTestSponsor entity)
        {
            _context.DoctorMedicalTestSponsors.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DoctorMedicalTestSponsor>> GetDoctorOffersAsync(int doctorId)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.MedicalTestProvider)
                .Include(x => x.CounterOffers)
                .Where(x => x.DoctorId == doctorId && !x.IsCounterOffer)
                .OrderByDescending(x => x.RequestedAtUtc)
                .ToListAsync();
        }

        public async Task<DoctorMedicalTestSponsor?> GetActiveSponsorshipAsync(
            int doctorId,
            MedicalTestProviderType type)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.MedicalTestProvider)
                    .ThenInclude(x => x.Offerings)
                        .ThenInclude(x => x.TestService)
                .FirstOrDefaultAsync(x => x.DoctorId == doctorId
                                       && x.ProviderType == type
                                       && x.Status == SponsorshipStatus.Active);
        }

        public async Task<bool> DoctorHasActiveSponsorAsync(int doctorId, MedicalTestProviderType type)
        {
            return await _context.DoctorMedicalTestSponsors
                .AnyAsync(x => x.DoctorId == doctorId
                            && x.ProviderType == type
                            && x.Status == SponsorshipStatus.Active);
        }

        public async Task<(int pending, int counter, int rejected)> GetProviderOfferCountsAsync(int providerId)
        {
            var pending = await _context.DoctorMedicalTestSponsors
                .CountAsync(x => x.MedicalTestProviderId == providerId
                              && !x.IsCounterOffer
                              && x.Status == SponsorshipStatus.Pending);

            var counter = await _context.DoctorMedicalTestSponsors
                .CountAsync(x => x.MedicalTestProviderId == providerId
                              && x.IsCounterOffer
                              && x.Status == SponsorshipStatus.Pending);

            var rejected = await _context.DoctorMedicalTestSponsors
                .CountAsync(x => x.MedicalTestProviderId == providerId
                              && !x.IsCounterOffer
                              && x.Status == SponsorshipStatus.Rejected);

            return (pending, counter, rejected);
        }

        public async Task<Dictionary<int, int>> GetPatientsSentCountPerDoctorAsync(int providerId)
        {
            return await _context.PatientReferrals
                .Include(x => x.Sponsorship)
                .Where(x => x.Sponsorship.MedicalTestProviderId == providerId)
                .GroupBy(x => x.DoctorId)
                .Select(g => new { DoctorId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.DoctorId, x => x.Count);
        }

        // =============== PAGINATED METHODS ===============

        public async Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersPagedAsync(
            int providerId,
            PaginationDto pagination)
        {
            var query = _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.CounterOffers)
                .Include(x => x.ParentOffer)
                .Where(x => x.MedicalTestProviderId == providerId && !x.IsCounterOffer);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestedAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResultDto<DoctorMedicalTestSponsor>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<DoctorMedicalTestSponsor>> GetDoctorOffersPagedAsync(
            int doctorId,
            PaginationDto pagination)
        {
            var query = _context.DoctorMedicalTestSponsors
                .Include(x => x.MedicalTestProvider)
                .Include(x => x.CounterOffers)
                .Where(x => x.DoctorId == doctorId && !x.IsCounterOffer);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestedAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResultDto<DoctorMedicalTestSponsor>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersByStatusPagedAsync(
            int providerId,
            StatusPaginationDto pagination)
        {
            var query = _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.CounterOffers)
                .Include(x => x.ParentOffer)
                .Where(x => x.MedicalTestProviderId == providerId && !x.IsCounterOffer);

            if (pagination.Status.HasValue)
            {
                query = query.Where(x => x.Status == pagination.Status.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestedAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResultDto<DoctorMedicalTestSponsor>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<PagedResultDto<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderPagedAsync(
            int providerId,
            PaginationDto pagination)
        {
            var query = _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.ParentOffer)
                .Include(x => x.CounterOffers)
                .Where(x => x.MedicalTestProviderId == providerId
                         && x.Status == SponsorshipStatus.Active);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestedAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResultDto<DoctorMedicalTestSponsor>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
        public async Task<PagedResultDto<DoctorMedicalTestSponsor>> GetProviderOffersByFilterPagedAsync(
    int providerId,
    OfferFilterStatus filterStatus,
    PaginationDto pagination)
        {
            IQueryable<DoctorMedicalTestSponsor> query;

            if (filterStatus == OfferFilterStatus.Counter)
            {
                // Counter offers: IsCounterOffer = true, Status = Pending
                query = _context.DoctorMedicalTestSponsors
                    .Include(x => x.Doctor)
                        .ThenInclude(d => d.specialization)
                    .Include(x => x.ParentOffer)
                    .Include(x => x.CounterOffers)
                    .Where(x => x.MedicalTestProviderId == providerId
                             && x.CounterOffers.Any() && x.CounterOffers.First().Status == SponsorshipStatus.Pending);
            }
            else if (filterStatus == OfferFilterStatus.Pending)
            {
                // Pending offers: IsCounterOffer = false, Status = Pending
                query = _context.DoctorMedicalTestSponsors
                    .Include(x => x.Doctor)
                        .ThenInclude(d => d.specialization)
                      .Include(x => x.CounterOffers)
                    .Where(x => x.MedicalTestProviderId == providerId
                             && !x.IsCounterOffer
                             && x.Status == SponsorshipStatus.Pending);
            }
            else // Rejected offers: IsCounterOffer = false, Status = Rejected
            {
                query = _context.DoctorMedicalTestSponsors
                    .Include(x => x.Doctor)
                        .ThenInclude(d => d.specialization)
                       .Include(x => x.CounterOffers)
                    .Where(x => x.MedicalTestProviderId == providerId
                             && !x.IsCounterOffer
                             && x.Status == SponsorshipStatus.Rejected);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.RequestedAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResultDto<DoctorMedicalTestSponsor>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
        public async Task<bool> DeletePendingOfferAsync(int sponsorshipId, int providerId)
        {
            var offer = await _context.DoctorMedicalTestSponsors
                .FirstOrDefaultAsync(x => x.Id == sponsorshipId
                                       && x.MedicalTestProviderId == providerId
                                       && !x.IsCounterOffer
                                       && x.Status == SponsorshipStatus.Pending);

            if (offer == null)
                return false;

            _context.DoctorMedicalTestSponsors.Remove(offer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForDoctorAsync(int doctorId)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.MedicalTestProvider)
                .Where(x => x.DoctorId == doctorId
                         && x.Status == SponsorshipStatus.Active)
                .ToListAsync();
        }
    }
}
