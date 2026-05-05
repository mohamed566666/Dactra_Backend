namespace Dactra.Repositories.Implementation
{
    public class PatientReferralRepository : IPatientReferralRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientReferralRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PatientReferral?> GetByIdAsync(int id)
        {
            return await _context.PatientReferrals
                .Where(x => x.Id == id)
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.Sponsorship)
                    .ThenInclude(x => x.MedicalTestProvider)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PatientReferral>> GetReferralsByProviderAsync(int providerId)
        {
            return await _context.PatientReferrals
                .Where(x => x.Sponsorship.MedicalTestProviderId == providerId)
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.Sponsorship)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .OrderByDescending(x => x.ReferredAtUtc)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientReferral>> GetReferralsByDoctorAsync(int doctorId)
        {
            return await _context.PatientReferrals
                .Where(x => x.DoctorId == doctorId)
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Sponsorship)
                    .ThenInclude(x => x.MedicalTestProvider)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .OrderByDescending(x => x.ReferredAtUtc)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<PatientReferral> CreateAsync(PatientReferral referral)
        {
            await _context.PatientReferrals.AddAsync(referral);
            await _context.SaveChangesAsync();
            return referral;
        }

        public async Task UpdateAsync(PatientReferral referral)
        {
            _context.PatientReferrals.Update(referral);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PatientDoctorCare>> GetActiveCarePatientsByDoctorAsync(int doctorId)
        {
            return await _context.PatientDoctorCares
                .Where(x => x.DoctorId == doctorId
                         && x.IsActive
                         && x.ExpiresAtUtc > DateTime.UtcNow)
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .OrderBy(x => x.Patient.FirstName)
                .ToListAsync();
        }

        public async Task<PagedResultDto<PatientReferral>> GetReferralsByProviderPagedAsync(
            int providerId,
            PaginationDto pagination,
            ReferralStatus? status = null)
        {
            var query = _context.PatientReferrals
                .Where(x => x.Sponsorship.MedicalTestProviderId == providerId);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.Sponsorship)
                    .ThenInclude(x => x.MedicalTestProvider)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .OrderByDescending(x => x.ReferredAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .AsSplitQuery()
                .ToListAsync();

            return new PagedResultDto<PatientReferral>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<int> GetUniquePatientReferralCountByDoctorAsync(int doctorId)
        {
            return await _context.PatientReferrals
                .Where(x => x.DoctorId == doctorId)
                .Select(x => x.PatientId)
                .Distinct()
                .CountAsync();
        }
    }
}