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
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.Sponsorship)
                    .ThenInclude(x => x.MedicalTestProvider)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PatientReferral>> GetReferralsByProviderAsync(int providerId)
        {
            return await _context.PatientReferrals
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.Sponsorship)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .Where(x => x.Sponsorship.MedicalTestProviderId == providerId)
                .OrderByDescending(x => x.ReferredAtUtc)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientReferral>> GetReferralsByDoctorAsync(int doctorId)
        {
            return await _context.PatientReferrals
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Sponsorship)
                    .ThenInclude(x => x.MedicalTestProvider)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .Where(x => x.DoctorId == doctorId)
                .OrderByDescending(x => x.ReferredAtUtc)
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
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Where(x => x.DoctorId == doctorId
                         && x.IsActive
                         && x.ExpiresAtUtc > DateTime.UtcNow)
                .OrderBy(x => x.Patient.FirstName)
                .ToListAsync();
        }

        public async Task<PagedResultDto<PatientReferral>> GetReferralsByProviderPagedAsync(
            int providerId,
            PaginationDto pagination,
            ReferralStatus? status = null)
        {
            var query = _context.PatientReferrals
                .Include(x => x.Patient)
                    .ThenInclude(p => p.User)
                .Include(x => x.Doctor)
                    .ThenInclude(d => d.specialization)
                .Include(x => x.Sponsorship)
                    .ThenInclude(x => x.MedicalTestProvider)
                .Include(x => x.ReferralServices)
                    .ThenInclude(x => x.ProviderOffering)
                        .ThenInclude(x => x.TestService)
                .Where(x => x.Sponsorship.MedicalTestProviderId == providerId);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.ReferredAtUtc)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResultDto<PatientReferral>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
    }
}