using Dactra.DTOs.DoctorSlotsDTOs;
using Microsoft.AspNetCore.Routing;
using NetTopologySuite.Index;

namespace Dactra.Services.Implementation
{
    public class DoctorSlotService : IDoctorSlotService
    {
        private readonly IGenericRepository<DoctorAvailabilitySlot> _repo;
        private readonly IHubContext<DoctorScheduleHub> _hub;
        private readonly IDoctorProfileRepository _doctorrepository;

        public DoctorSlotService(
            IGenericRepository<DoctorAvailabilitySlot> repo,
            IHubContext<DoctorScheduleHub> hub,
            IDoctorProfileRepository doctorrepository)
        {
            _repo = repo;
            _hub = hub;
            _doctorrepository = doctorrepository;
        }

        private async Task SaveSlotsInternalAsync(
            int doctorId,
            Dictionary<string, List<SlotItemDto>> slots,
            SlotType slotType)
        {
            var workingHours = await GetWorkingHoursAsync(doctorId);
            var entry = slotType == SlotType.Online
                ? workingHours.Online
                : workingHours.InPerson;

            if (!entry.WorkingStartTime.HasValue || !entry.WorkingEndTime.HasValue)
                throw new InvalidOperationException(
                    $"{slotType} working hours are not configured yet. " +
                    $"Please set working hours first.");

            foreach (var kvp in slots)
            {
                if (!DateTime.TryParseExact(kvp.Key, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dayUtc))
                    throw new Exception($"Invalid date format: {kvp.Key}");

                dayUtc = dayUtc.Date;

                await DeleteSlotsByDayAndTypeAsync(doctorId, dayUtc, slotType);

                foreach (var slot in kvp.Value)
                {
                    var slotTime = slot.SlotTime.TimeOfDay;

                    //if (!IsWithinWorkingHours(slotTime, entry))
                    //    throw new Exception(
                    //        $"Slot time {slot.SlotTime} is outside {slotType} working hours " +
                    //        $"({entry.WorkingStartTime:hh\\:mm} - {entry.WorkingEndTime:hh\\:mm})");

                    var isBooked = await _repo.FindAsync(
                        s => s.DoctorId == doctorId &&
                             s.SlotDateTimeUtc == slot.SlotTime &&
                             s.SlotType == slotType &&
                             s.IsBooked == true);

                    if (isBooked.Any()) continue;

                    var existingSlot = await _repo.FindAsync(
                        s => s.DoctorId == doctorId &&
                             s.SlotDateTimeUtc == slot.SlotTime);

                    if (existingSlot.Any()) continue;

                        await _repo.AddAsync(new DoctorAvailabilitySlot
                    {
                        DoctorId = doctorId,
                        SlotDateTimeUtc = slot.SlotTime,
                        IsBooked = slot.IsBooked,
                        SlotType = slotType,
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }

            await _repo.SaveChangesAsync();
            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new { DoctorId = doctorId, SlotType = slotType.ToString() });
        }

        public Task SaveInPersonSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots)
            => SaveSlotsInternalAsync(doctorId, slots, SlotType.InPerson);

        public Task SaveOnlineSlotsAsync(int doctorId, Dictionary<string, List<SlotItemDto>> slots)
            => SaveSlotsInternalAsync(doctorId, slots, SlotType.Online);

        // ────────────────────────────────────────────────────────────────────────────

        public Task<DoctorSlotsDto> GetAllInPersonSlotsAsync(int doctorId)
            => GetAllSlotsInternalAsync(doctorId, SlotType.InPerson);

        public Task<DoctorSlotsDto> GetAllOnlineSlotsAsync(int doctorId)
            => GetAllSlotsInternalAsync(doctorId, SlotType.Online);

        public Task<DoctorSlotsDto> GetInPersonSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc)
            => GetSlotsRangeInternalAsync(doctorId, fromUtc, toUtc, SlotType.InPerson);

        public Task<DoctorSlotsDto> GetOnlineSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc)
            => GetSlotsRangeInternalAsync(doctorId, fromUtc, toUtc, SlotType.Online);

        public Task<DoctorFreeSlotsDto> GetAllFreeInPersonSlotsAsync(int doctorId)
            => GetAllFreeSlotsInternalAsync(doctorId, SlotType.InPerson);

        public Task<DoctorFreeSlotsDto> GetAllFreeOnlineSlotsAsync(int doctorId)
            => GetAllFreeSlotsInternalAsync(doctorId, SlotType.Online);

        public Task<DoctorFreeSlotsDto> GetFreeInPersonSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc)
            => GetFreeRangeInternalAsync(doctorId, fromUtc, toUtc, SlotType.InPerson);

        public Task<DoctorFreeSlotsDto> GetFreeOnlineSlotsAsync(int doctorId, DateTime fromUtc, DateTime toUtc)
            => GetFreeRangeInternalAsync(doctorId, fromUtc, toUtc, SlotType.Online);

        // ─── Private core implementations ───────────────────────────────────────────

        private async Task<DoctorSlotsDto> GetAllSlotsInternalAsync(int doctorId, SlotType slotType)
        {
            var currentTypeSlots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotType == slotType &&
                x.SlotDateTimeUtc >= DateTime.UtcNow &&
                x.IsReserved == false);

            var workingHours = await GetWorkingHoursAsync(doctorId);
            var entry = slotType == SlotType.Online
                ? workingHours.Online
                : workingHours.InPerson;

            if (!entry.WorkingStartTime.HasValue || !entry.WorkingEndTime.HasValue)
                return MapToDoctorSlotsDto(currentTypeSlots);

            var otherType = slotType == SlotType.Online ? SlotType.InPerson : SlotType.Online;

            var otherTypeBookedSlots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotType == otherType &&
                x.IsBooked == true &&
                x.SlotDateTimeUtc >= DateTime.UtcNow);

            var filteredOtherSlots = otherTypeBookedSlots
                .Where(slot =>
                {
                    var slotTime = slot.SlotDateTimeUtc.TimeOfDay;
                    return slotTime >= entry.WorkingStartTime.Value &&
                           slotTime <= entry.WorkingEndTime.Value;
                })
                .Select(slot => new DoctorAvailabilitySlot
                {
                    Id = slot.Id,
                    DoctorId = slot.DoctorId,
                    SlotDateTimeUtc = slot.SlotDateTimeUtc,
                    IsBooked = true,
                    SlotType = slotType,
                    IsReserved = slot.IsReserved
                });

            var allSlots = currentTypeSlots.Concat(filteredOtherSlots);

            return MapToDoctorSlotsDto(allSlots);
        }

        private async Task<DoctorSlotsDto> GetSlotsRangeInternalAsync(
            int doctorId, DateTime fromUtc, DateTime toUtc, SlotType slotType)
        {
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotType == slotType &&
                x.SlotDateTimeUtc >= fromUtc &&
                x.SlotDateTimeUtc <= toUtc &&
                x.IsReserved == false);
            return MapToDoctorSlotsDto(slots);
        }

        private async Task<DoctorFreeSlotsDto> GetAllFreeSlotsInternalAsync(int doctorId, SlotType slotType)
        {
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotType == slotType &&
                !x.IsBooked &&
                x.IsReserved == false);
            return MapToDoctorFreeSlotsDto(slots);
        }

        private async Task<DoctorFreeSlotsDto> GetFreeRangeInternalAsync(
            int doctorId, DateTime fromUtc, DateTime toUtc, SlotType slotType)
        {
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotType == slotType &&
                !x.IsBooked &&
                x.SlotDateTimeUtc >= fromUtc &&
                x.SlotDateTimeUtc <= toUtc &&
                x.IsReserved == false);
            return MapToDoctorFreeSlotsDto(slots);
        }

        // ─── Delete ──────────────────────────────────────────────────────────────────

        private async Task DeleteSlotsByDayAndTypeAsync(int doctorId, DateTime dayUtc, SlotType slotType)
        {
            dayUtc = dayUtc.Date;
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotType == slotType &&
                x.SlotDateTimeUtc.Date == dayUtc);

            foreach (var slot in slots)
                if (!slot.IsBooked)
                    _repo.Delete(slot);

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteSlotsByDayAsync(int doctorId, DateTime dayUtc)
        {
            dayUtc = dayUtc.Date;
            var slots = await _repo.FindAsync(x =>
                x.DoctorId == doctorId &&
                x.SlotDateTimeUtc.Date == dayUtc);

            foreach (var slot in slots)
                if (!slot.IsBooked)
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

        // ─── Working Hours ────────────────────────────────────────────────────────────

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

                 await DeleteUnbookedSlotsByTypeAsync(doctorId, workingHours.Type);

                return await _doctorrepository.GetWorkingHoursAsync(doctorId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating working hours for doctor {doctorId}", ex);
            }
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────────

        private static bool IsWithinWorkingHours(TimeSpan slotTime, WorkingHoursEntryDTO entry)
            => entry.WorkingStartTime.HasValue &&
               entry.WorkingEndTime.HasValue &&
               slotTime >= entry.WorkingStartTime.Value &&
               slotTime <= entry.WorkingEndTime.Value;

        private static DoctorSlotsDto MapToDoctorSlotsDto(IEnumerable<DoctorAvailabilitySlot> slots)
        {
            var grouped = slots
                .OrderBy(s => s.SlotDateTimeUtc)
                .GroupBy(s => s.SlotDateTimeUtc.ToString("dd-MM-yyyy"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => new SlotItemDto
                    {
                        SlotTime = s.SlotDateTimeUtc,
                        IsBooked = s.IsBooked
                    }).ToList());
            return new DoctorSlotsDto { Slots = grouped };
        }

        private static DoctorFreeSlotsDto MapToDoctorFreeSlotsDto(IEnumerable<DoctorAvailabilitySlot> slots)
        {
            var grouped = slots
                .OrderBy(s => s.SlotDateTimeUtc)
                .GroupBy(s => s.SlotDateTimeUtc.ToString("dd-MM-yyyy"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(s => new FreeSlotDto
                    {
                        SlotId = s.Id,
                        SlotTime = TimeOnly.FromDateTime(s.SlotDateTimeUtc),
                        IsBooked = s.IsBooked
                    }).ToList());
            return new DoctorFreeSlotsDto { Slots = grouped };
        }

        public Task<int> GetDoctorIdBySlotId(int slotId)
        {
            var slot = _repo.GetByIdAsync(slotId).Result;
            if (slot == null)
                throw new Exception($"Slot with id {slotId} not found");
            return Task.FromResult(slot.DoctorId);
        }

        private async Task DeleteUnbookedSlotsByTypeAsync(int doctorId, SlotType slotType)
        {
            var unbookedSlots = await _repo.FindAsync(s =>
                s.DoctorId == doctorId &&
                s.SlotType == slotType &&
                !s.IsBooked);

            if (!unbookedSlots.Any())
                return;

            foreach (var slot in unbookedSlots)
                _repo.Delete(slot);

            await _repo.SaveChangesAsync();

            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new
                {
                    DoctorId = doctorId,
                    WorkingHoursUpdated = true,
                    SlotType = slotType.ToString(),
                    DeletedUnbookedSlotsCount = unbookedSlots.Count(),
                    Message = $"All unbooked {slotType} slots have been deleted due to working hours update."
                });
        }

        public async Task<int> DeleteUnbookedPastSlotsByDoctorAsync(int doctorId)
        {
            var nowUtc = DateTime.UtcNow;

            var pastUnbookedSlots = await _repo.FindAsync(s =>
                s.DoctorId == doctorId &&
                s.SlotDateTimeUtc >= nowUtc &&
                !s.IsBooked);

            if (!pastUnbookedSlots.Any())
                return 0;

            foreach (var slot in pastUnbookedSlots)
                _repo.Delete(slot);

            await _repo.SaveChangesAsync();

            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new { DoctorId = doctorId, DeletedPastUnbooked = true });

            return pastUnbookedSlots.Count();
        }

        public async Task<int> DeleteAllPastSlotsByDoctorAsync(int doctorId)
        {
            var nowUtc = DateTime.UtcNow;

            var pastSlots = await _repo.FindAsync(s =>
                s.DoctorId == doctorId &&
                s.SlotDateTimeUtc < nowUtc);

            if (!pastSlots.Any())
                return 0;

            foreach (var slot in pastSlots)
                _repo.Delete(slot);

            await _repo.SaveChangesAsync();

            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new { DoctorId = doctorId, DeletedAllPast = true });

            return pastSlots.Count();
        }

        public async Task<int> DeleteSlotsOutsideWorkingHoursAsync(int doctorId, SlotType slotType)
        {
            var workingHours = await GetWorkingHoursAsync(doctorId);

            var entry = slotType == SlotType.Online
                ? workingHours.Online
                : workingHours.InPerson;

            if (!entry.WorkingStartTime.HasValue || !entry.WorkingEndTime.HasValue)
                throw new InvalidOperationException(
                    $"{slotType} working hours are not configured for doctor {doctorId}");

            var allSlots = await _repo.FindAsync(s =>
                s.DoctorId == doctorId &&
                s.SlotType == slotType);

            var slotsToDelete = new List<DoctorAvailabilitySlot>();

            foreach (var slot in allSlots)
            {
                var slotTime = slot.SlotDateTimeUtc.TimeOfDay;

                if (slotTime < entry.WorkingStartTime.Value ||
                    slotTime > entry.WorkingEndTime.Value)
                {
                    if (!slot.IsBooked)
                        slotsToDelete.Add(slot);
                }
            }

            if (!slotsToDelete.Any())
                return 0;

            foreach (var slot in slotsToDelete)
                _repo.Delete(slot);

            await _repo.SaveChangesAsync();

            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new
                {
                    DoctorId = doctorId,
                    SlotType = slotType.ToString(),
                    DeletedOutsideWorkingHours = slotsToDelete.Count
                });

            return slotsToDelete.Count;
        }

        public async Task<int> DeleteAllSlotsOutsideWorkingHoursAsync(int doctorId)
        {
            var onlineCount = await DeleteSlotsOutsideWorkingHoursAsync(doctorId, SlotType.Online);
            var inPersonCount = await DeleteSlotsOutsideWorkingHoursAsync(doctorId, SlotType.InPerson);

            return onlineCount + inPersonCount;
        }

        public async Task<int> DeleteAllFreeSlotsByDoctorAsync(int doctorId)
        {
            var freeSlots = await _repo.FindAsync(s =>
                s.DoctorId == doctorId &&
                !s.IsBooked);

            if (!freeSlots.Any())
                return 0;

            foreach (var slot in freeSlots)
                _repo.Delete(slot);

            await _repo.SaveChangesAsync();

            await _hub.Clients.Group($"DoctorSchedule_{doctorId}")
                .SendAsync("SlotsUpdated", new
                {
                    DoctorId = doctorId,
                    DeletedAllFreeSlots = true,
                    DeletedCount = freeSlots.Count(),
                    Message = $"All {freeSlots.Count()} free slot(s) have been deleted."
                });

            return freeSlots.Count();
        }
    }
}
