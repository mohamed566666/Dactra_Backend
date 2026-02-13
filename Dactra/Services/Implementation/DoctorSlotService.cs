using Dactra.DTOs.DoctorSlotsDTOs;

namespace Dactra.Services.Implementation
{
    public class DoctorSlotService : IDoctorSlotService
    {
        private readonly IGenericRepository<DoctorAvailabilitySlot> _repo;
        private readonly IHubContext<DoctorScheduleHub> _hub;
        private readonly IDoctorProfileRepository _doctorrepository;

        public DoctorSlotService(
            IGenericRepository<DoctorAvailabilitySlot> repo,
            IHubContext<DoctorScheduleHub> hub ,
            IDoctorProfileRepository doctorrepository)
        {
            _repo = repo;
            _hub = hub;
            _doctorrepository = doctorrepository;
        }

        public async Task SaveSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots)
        {
            foreach (var kvp in slots)
            {
                if (!DateTime.TryParse(kvp.Key, out var dayUtc))
                    throw new Exception($"Invalid date format: {kvp.Key}");

                dayUtc = dayUtc.Date;
                await DeleteSlotsByDayAsync(doctorId, dayUtc);

                foreach (var slot in kvp.Value)
                {
                    var timeParts = slot.Time.Split(':');
                    if (!int.TryParse(timeParts[0], out var hour) || !int.TryParse(timeParts[1], out var minute))
                        throw new Exception($"Invalid time format: {slot.Time}");

                    var slotDateTimeUtc = new DateTime(
                        dayUtc.Year, dayUtc.Month, dayUtc.Day,
                        hour, minute, 0, DateTimeKind.Utc);

                    await _repo.AddAsync(new DoctorAvailabilitySlot
                    {
                        DoctorId = doctorId,
                        SlotDateTimeUtc = slotDateTimeUtc,
                        IsBooked = slot.IsBooked,
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }

            await _repo.SaveChangesAsync();
            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new { DoctorId = doctorId });
        }

        public async Task<List<DoctorSlotResponseDto>> GetSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc)
        {
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotDateTimeUtc >= fromUtc &&
                x.SlotDateTimeUtc <= toUtc);

            return slots
                .OrderBy(s => s.SlotDateTimeUtc)
                .Select(s => new DoctorSlotResponseDto
                {
                    SlotId = s.Id,
                    SlotDateTimeUtc = s.SlotDateTimeUtc,
                    IsBooked = s.IsBooked
                })
                .ToList();
        }

        public async Task<List<DoctorSlotResponseDto>> GetAllSlotsAsync(int doctorId)
        {
            var slots = await _repo.FindAsync(x => x.DoctorId == doctorId);
            return slots
                .OrderBy(s => s.SlotDateTimeUtc)
                .Select(s => new DoctorSlotResponseDto
                {
                    SlotId = s.Id,
                    SlotDateTimeUtc = s.SlotDateTimeUtc,
                    IsBooked = s.IsBooked
                })
                .ToList();
        }

        public async Task DeleteSlotsByDayAsync(int doctorId, DateTime dayUtc)
        {
            dayUtc = dayUtc.Date;
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotDateTimeUtc.Date == dayUtc);

            foreach (var slot in slots)
                _repo.Delete(slot);

            await _repo.SaveChangesAsync();
            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new { DoctorId = doctorId });
        }

        public async Task DeleteSlotAsync(int doctorId, int slotId)
        {
            var slot = await _repo.GetByIdAsync(slotId);
            if (slot == null || slot.DoctorId != doctorId) return;

            _repo.Delete(slot);
            await _repo.SaveChangesAsync();
            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new { DoctorId = doctorId });
        }

        public async Task<WorkingHoursResponseDTO> GetWorkingHoursAsync(int doctorId)
        {
            try
            {
                return await _doctorrepository.GetWorkingHoursAsync(doctorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving working hours for doctor {doctorId}", ex);
            }
        }

        public async Task<WorkingHoursResponseDTO> UpdateWorkingHoursAsync(int doctorId, WorkingHoursDTO workingHours)
        {
            try
            {
                var updated = await _doctorrepository.UpdateWorkingHoursAsync(doctorId, workingHours);
                if (!updated)
                    throw new InvalidOperationException($"Failed to update working hours for doctor {doctorId}");
                var result = await _doctorrepository.GetWorkingHoursAsync(doctorId);
                return result;
            }
            catch (Exception ex)
            { 
                throw new Exception($"Error updating working hours for doctor {doctorId}", ex);
            }
        }
    }
}
