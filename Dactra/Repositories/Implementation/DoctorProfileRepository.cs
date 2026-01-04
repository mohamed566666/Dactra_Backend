namespace Dactra.Repositories.Implementation
{
    public class DoctorProfileRepository : GenericRepository<DoctorProfile> , IDoctorProfileRepository
    {
        public DoctorProfileRepository(ApplicationDbContext context) : base(context)
        {

        }
        public override async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorProfile>> GetApprovedDoctorsAsync()
        {
            return await _context.Doctors
                .Where(m => m.IsApproved)
                .OrderByDescending(M => M.Avg_Rating)
                .ToListAsync();
        }

        public override async Task<DoctorProfile?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile?> GetByUserEmail(string email)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.User.Email == email);
        }

        public async Task<DoctorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IEnumerable<DoctorProfile>> GetdisApprovedDoctorsAsync()
        {
            return await _context.Doctors
                .Where(m => !m.IsApproved)
                .ToListAsync();
        }

        public async Task<(IEnumerable<DoctorProfile> doctors, int totalCount)> GetFilteredDoctorsAsync(DoctorFilterDTO filter)
        {
            double FuzzyThreshold = 0.70;
            int PrefixLength = 2;
            int MaxCandidatesHardCap = 2000;
        filter.PageNumber = Math.Max(1, filter.PageNumber);
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
            var baseQuery = _context.Doctors
                .Where(d => d.IsApproved)
                .Include(d => d.User)
                .Include(d => d.specialization)
                .AsQueryable();
            if (filter.SpecializationId.HasValue)
                baseQuery = baseQuery.Where(d => d.SpecializationId == filter.SpecializationId.Value);
            if (filter.Gender.HasValue)
                baseQuery = baseQuery.Where(d => d.Gender == filter.Gender.Value);
            bool useFuzzy = !string.IsNullOrWhiteSpace(filter.SearchTerm);
            string normalized = useFuzzy ? filter.SearchTerm.Trim().ToLower() : null;
            if (!useFuzzy)
            {
                var totalCount = await baseQuery.CountAsync();

                if (filter.SortedByRating.HasValue && filter.SortedByRating.Value)
                    baseQuery = baseQuery.OrderByDescending(d => d.Avg_Rating);
                else
                    baseQuery = baseQuery.OrderBy(d => d.FirstName).ThenBy(d => d.LastName);
                var doctors = await baseQuery
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return (doctors, totalCount);
            }
            var prefix = normalized.Length >= PrefixLength ? normalized.Substring(0, PrefixLength) : normalized;
            IQueryable<DoctorProfile> stageAQuery = baseQuery.Where(d =>
                ((d.FirstName ?? string.Empty) + " " + (d.LastName ?? string.Empty)).ToLower().Contains(prefix) ||
                (d.FirstName ?? string.Empty).ToLower().Contains(prefix) ||
                (d.LastName ?? string.Empty).ToLower().Contains(prefix)
            );
            stageAQuery = stageAQuery.OrderByDescending(d => d.Avg_Rating);
            var candidates = await stageAQuery
                .AsNoTracking()
                .Take(Math.Min(500, MaxCandidatesHardCap))
                .ToListAsync();
            if (candidates.Count == 0)
            {
                IQueryable<DoctorProfile> stageBQuery = baseQuery.Where(d =>
                    ((d.FirstName ?? string.Empty) + " " + (d.LastName ?? string.Empty)).ToLower().Contains(normalized) ||
                    (d.FirstName ?? string.Empty).ToLower().Contains(normalized) ||
                    (d.LastName ?? string.Empty).ToLower().Contains(normalized)
                ).OrderByDescending(d => d.Avg_Rating);
                candidates = await stageBQuery
                    .AsNoTracking()
                    .Take(Math.Min(1000, MaxCandidatesHardCap))
                    .ToListAsync();
            }
            if (candidates.Count == 0)
            {
                candidates = await baseQuery
                    .AsNoTracking()
                    .OrderByDescending(d => d.Avg_Rating)
                    .Take(MaxCandidatesHardCap)
                    .ToListAsync();
            }   
            var scored = candidates.Select(d =>
            {
                var fullName = $"{d.FirstName} {d.LastName}".Trim().ToLower();
                double bestScore = 0;
                bestScore = Math.Max(bestScore, FuzzyMatcher.SimilarityScore(fullName, normalized));
                bestScore = Math.Max(bestScore, FuzzyMatcher.SimilarityScore((d.FirstName ?? string.Empty).ToLower(), normalized));
                bestScore = Math.Max(bestScore, FuzzyMatcher.SimilarityScore((d.LastName ?? string.Empty).ToLower(), normalized));
                string matchLevel =
                    bestScore >= 0.80 ? "HIGH" :
                    bestScore >= 0.55 ? "MEDIUM" :
                    "LOW";
                return new { Doctor = d, Score = bestScore, Level = matchLevel };
            }).Where(x => x.Level != "LOW")
            .OrderByDescending(x => x.Level == "HIGH")
            .ThenByDescending(x => x.Score)
            .ThenByDescending(x => x.Doctor.Avg_Rating)
            .ThenBy(x => x.Doctor.FirstName)
            .ThenBy(x => x.Doctor.LastName)
            .ToList();
            if (scored.Count == 0)
            {
                const double relaxedThreshold = 0.45;
                scored = candidates.Select(d =>
                {
                    var fullName = $"{d.FirstName} {d.LastName}".Trim().ToLower();
                    double bestScore = 0;
                    bestScore = Math.Max(bestScore, FuzzyMatcher.SimilarityScore(fullName, normalized));
                    bestScore = Math.Max(bestScore, FuzzyMatcher.SimilarityScore((d.FirstName ?? string.Empty).ToLower(), normalized));
                    bestScore = Math.Max(bestScore, FuzzyMatcher.SimilarityScore((d.LastName ?? string.Empty).ToLower(), normalized));
                    string matchLevel =
                        bestScore >= 0.80 ? "HIGH" :
                        bestScore >= 0.60 ? "MEDIUM" :
                        "LOW";
                    return new { Doctor = d, Score = bestScore, Level = matchLevel };
                })
                .Where(x => x.Score >= relaxedThreshold)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Doctor.Avg_Rating)
                .ThenBy(x => x.Doctor.FirstName)
                .ThenBy(x => x.Doctor.LastName)
                .ToList();
            }
            var totalFuzzyMatches = scored.Count;
            var pageIndex = filter.PageNumber - 1;
            var pageSize = filter.PageSize;
            var paged = scored
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(x => x.Doctor)
                .ToList();
            return (paged, totalFuzzyMatches);
        }
    }
}
