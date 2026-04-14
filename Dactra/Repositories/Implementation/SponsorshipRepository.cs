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
            SponsorshipStatus status)
        {



            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.CounterOffers)
                .Where(x => x.MedicalTestProviderId == providerId
                         && !x.IsCounterOffer
                         && x.Status == status)
                .OrderByDescending(x => x.RequestedAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorMedicalTestSponsor>> GetActiveSponsorsForProviderAsync(int providerId)
        {
            return await _context.DoctorMedicalTestSponsors
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
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
    }
}
