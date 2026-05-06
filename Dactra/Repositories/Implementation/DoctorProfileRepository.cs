using Dactra.DTOs.DoctorSlotsDTOs;

namespace Dactra.Repositories.Implementation
{
    public class DoctorProfileRepository : GenericRepository<DoctorProfile>, IDoctorProfileRepository
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
                .Where(m => m.approvalStatus == ApprovalStatus.approved)
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
                .Where(m => m.approvalStatus == ApprovalStatus.rejected)
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
                .Where(d => d.approvalStatus == ApprovalStatus.approved)
                
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
                var doctorIds = await baseQuery
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(d => d.Id)
                    .ToListAsync();

                var doctors = await _context.Doctors
                    .Where(d => doctorIds.Contains(d.Id))
                    .Include(d => d.User)
                    .Include(d => d.specialization)
                    
                    .ToListAsync();

                return (doctors, totalCount);
            }
            var prefix = normalized.Length >= PrefixLength ? normalized.Substring(0, PrefixLength) : normalized;
            IQueryable<DoctorProfile> stageAQuery = baseQuery.Where(d =>
                (d.FirstName != null && d.FirstName.ToLower().Contains(prefix)) ||
                (d.LastName != null && d.LastName.ToLower().Contains(prefix))
            );
            stageAQuery = stageAQuery.OrderByDescending(d => d.Avg_Rating);
            var candidates = await stageAQuery
                .Take(Math.Min(500, MaxCandidatesHardCap))
                .Select(d => new { d.Id, d.FirstName, d.LastName, d.Avg_Rating })
                .ToListAsync();
            if (candidates.Count == 0)
            {
                IQueryable<DoctorProfile> stageBQuery = baseQuery.Where(d =>
                    (d.FirstName != null && d.FirstName.ToLower().Contains(normalized)) ||
                    (d.LastName != null && d.LastName.ToLower().Contains(normalized))
                ).OrderByDescending(d => d.Avg_Rating);
                candidates = await stageBQuery
                    .Take(Math.Min(1000, MaxCandidatesHardCap))
                    .Select(d => new { d.Id, d.FirstName, d.LastName, d.Avg_Rating })
                    .ToListAsync();
            }
            if (candidates.Count == 0)
            {
                candidates = await baseQuery
                    .OrderByDescending(d => d.Avg_Rating)
                    .Take(MaxCandidatesHardCap)
                    .Select(d => new { d.Id, d.FirstName, d.LastName, d.Avg_Rating })
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
                return new { DoctorId = d.Id, Score = bestScore, Level = matchLevel, AvgRating = d.Avg_Rating };
            }).Where(x => x.Level != "LOW")
            .OrderByDescending(x => x.Level == "HIGH")
            .ThenByDescending(x => x.Score)
            .ThenByDescending(x => x.AvgRating)
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
                    return new { DoctorId = d.Id, Score = bestScore, Level = matchLevel, AvgRating = d.Avg_Rating };
                })
                .Where(x => x.Score >= relaxedThreshold)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.AvgRating)
                .ToList();
            }
            var totalFuzzyMatches = scored.Count;
            var pageIndex = filter.PageNumber - 1;
            var pageSize = filter.PageSize;
            var pagedIds = scored
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(x => x.DoctorId)
                .ToList();
            var pagedDoctors = await _context.Doctors
                .Where(d => pagedIds.Contains(d.Id))
                .Include(d => d.User)
                .Include(d => d.specialization)
                
                .ToListAsync();
            var paged = pagedIds.Select(id => pagedDoctors.First(d => d.Id == id)).ToList();
            return (paged, totalFuzzyMatches);
        }
        public async Task<bool> UpdateWorkingHoursAsync(int doctorId, WorkingHoursDTO workingHours)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
                return false;

            if (!TimeSpan.TryParseExact(workingHours.WorkingStartTime, "hh\\:mm",
                CultureInfo.InvariantCulture, out var startTime))
                throw new InvalidOperationException("Invalid start time format. Use HH:mm (24-hour)");

            if (!TimeSpan.TryParseExact(workingHours.WorkingEndTime, "hh\\:mm",
                CultureInfo.InvariantCulture, out var endTime))
                throw new InvalidOperationException("Invalid end time format. Use HH:mm (24-hour)");

            //if (startTime >= endTime)
            //    throw new InvalidOperationException("Start time must be before end time");

            if (workingHours.ConsultationDurationMinutes <= 0)
                throw new InvalidOperationException("Consultation duration must be greater than 0");

            var totalWorkingMinutes = GetTotalWorkingMinutes(startTime, endTime);
            if (workingHours.ConsultationDurationMinutes > totalWorkingMinutes)
            {
                throw new InvalidOperationException(
                    $"Consultation duration ({workingHours.ConsultationDurationMinutes} minutes) " +
                    $"cannot be longer than total working hours period " +
                    $"({startTime:hh\\:mm} - {endTime:hh\\:mm}) which is {totalWorkingMinutes} minutes.");
            }

            if (workingHours.Type == SlotType.Online)
            {
                if (doctor.WorkingStartTime.HasValue && doctor.WorkingEndTime.HasValue)
                    ValidateNoOverlap(
                        newStart: startTime,
                        newEnd: endTime,
                        existingStart: doctor.WorkingStartTime.Value,
                        existingEnd: doctor.WorkingEndTime.Value,
                        newType: "Online",
                        existingType: "InPerson");

                doctor.OnlineWorkingStartTime = startTime;
                doctor.OnlineWorkingEndTime = endTime;
                doctor.OnlineConsultationDurationMinutes = workingHours.ConsultationDurationMinutes;
                doctor.OnlineConsultationPrice = workingHours.ConsultationPrice;
            }
            else
            {
                if (doctor.OnlineWorkingStartTime.HasValue && doctor.OnlineWorkingEndTime.HasValue)
                    ValidateNoOverlap(
                        newStart: startTime,
                        newEnd: endTime,
                        existingStart: doctor.OnlineWorkingStartTime.Value,
                        existingEnd: doctor.OnlineWorkingEndTime.Value,
                        newType: "InPerson",
                        existingType: "Online");

                doctor.WorkingStartTime = startTime;
                doctor.WorkingEndTime = endTime;
                doctor.ConsultationDurationMinutes = workingHours.ConsultationDurationMinutes;
                doctor.ConsultationPrice = workingHours.ConsultationPrice;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private double GetTotalWorkingMinutes(TimeSpan start, TimeSpan end)
        {
            if (start < end)
            {
                return (end - start).TotalMinutes;
            }
            else
            {
                var minutesUntilMidnight = (TimeSpan.FromHours(24) - start).TotalMinutes;
                var minutesAfterMidnight = end.TotalMinutes;
                return minutesUntilMidnight + minutesAfterMidnight;
            }
        }


        private static void ValidateNoOverlap(
            TimeSpan newStart, TimeSpan newEnd,
            TimeSpan existingStart, TimeSpan existingEnd,
            string newType, string existingType)
        {
            bool overlaps = newStart < existingEnd && newEnd > existingStart;
            if (overlaps)
                throw new InvalidOperationException(
                    $"{newType} working hours ({newStart:hh\\:mm} - {newEnd:hh\\:mm}) " +
                    $"overlap with {existingType} working hours " +
                    $"({existingStart:hh\\:mm} - {existingEnd:hh\\:mm}). " +
                    $"Working hours must not overlap.");
        }

        public async Task<WorkingHoursResponseDTO> GetWorkingHoursAsync(int doctorId)
        {
            var response = await _context.Doctors
                .Where(d => d.Id == doctorId)
                .Select(d => new WorkingHoursResponseDTO
                {
                    InPerson = new WorkingHoursEntryDTO
                    {
                        WorkingStartTime = d.WorkingStartTime,
                        WorkingEndTime = d.WorkingEndTime,
                        ConsultationDurationMinutes = d.ConsultationDurationMinutes,
                        ConsultationPrice = d.ConsultationPrice
                    },
                    Online = new WorkingHoursEntryDTO
                    {
                        WorkingStartTime = d.OnlineWorkingStartTime,
                        WorkingEndTime = d.OnlineWorkingEndTime,
                        ConsultationDurationMinutes = d.OnlineConsultationDurationMinutes,
                        ConsultationPrice = d.OnlineConsultationPrice
                    }
                })
                
                .FirstOrDefaultAsync();

            if (response == null)
                throw new KeyNotFoundException($"Doctor with ID {doctorId} not found");

            return response;
        }
    }
}