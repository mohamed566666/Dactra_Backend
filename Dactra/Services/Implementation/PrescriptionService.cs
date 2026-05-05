using Dactra.DTOs.PrescriptionDTOs;

namespace Dactra.Services.Implementation
{
    public class PrescriptionService  : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepo;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepo,
            ApplicationDbContext context,
            ILogger<PrescriptionService> logger)
        {
            _prescriptionRepo = prescriptionRepo;
            _context = context;
            _logger = logger;
        }

        #region Create

        public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(
            CreatePrescriptionRequestDto dto, int doctorId)
        {
            var appointment = await _prescriptionRepo.GetAppointmentWithDetailsAsync(dto.AppointmentId);

            if (appointment is null)
                throw new InvalidOperationException("Appointment not found");
            if (appointment.Slot.DoctorId != doctorId)
                throw new UnauthorizedAccessException("You are not authorized to write a prescription for this appointment");
            if (appointment.Status != AppointmentStatus.Confirmed &&
                appointment.Status != AppointmentStatus.Completed)
            {
                throw new InvalidOperationException("Cannot write a prescription for this appointment status");
            }
            var alreadyExists = await _prescriptionRepo.ExistsForAppointmentAsync(dto.AppointmentId);
            if (alreadyExists)
                throw new InvalidOperationException("A prescription already exists for this appointment");
            var prescription = new Prescription
            {
                Diagnosis = dto.Diagnosis,
                AppointmentId = dto.AppointmentId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var medDto in dto.Medicines)
            {
                var firstDoseTime = ParseTimeString(medDto.FirstDoseTime);
                ValidateMedicineDto(medDto, firstDoseTime);
                var medicine = new PrescriptionMedicine
                {
                    Name = medDto.Name,
                    Dose = medDto.Dose,
                    Duration = medDto.Duration,
                    TimesPerDay = medDto.TimesPerDay,
                    WhenToTake = medDto.WhenToTake,
                    FirstDoseTime = firstDoseTime,
                    DoseTimes = CalculateDoseTimes(firstDoseTime, medDto.TimesPerDay)
                };

                prescription.Medicines.Add(medicine);
            }

            var created = await _prescriptionRepo.CreateAsync(prescription);

            var loaded = await _prescriptionRepo.GetByIdAsync(created.Id);

            return MapToResponseDto(loaded!);
        }

        #endregion

        #region Get By Appointment

        public async Task<PrescriptionResponseDto?> GetByAppointmentIdAsync(
            int appointmentId, int userId, string userRole)
        {
            var prescription = await _prescriptionRepo.GetByAppointmentIdAsync(appointmentId);

            if (prescription is null)
                return null;

            if (userRole == "Doctor")
            {
                if (prescription.Appointment.Slot.DoctorId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to view this prescription");
            }
            else if (userRole == "Patient")
            {
                if (prescription.Appointment.PatientId != userId)
                    throw new UnauthorizedAccessException("You are not authorized to view this prescription");
            }

            return MapToResponseDto(prescription);
        }

        #endregion

        #region Update

        public async Task<PrescriptionResponseDto?> UpdatePrescriptionAsync(
            int prescriptionId, UpdatePrescriptionRequestDto dto, int doctorId)
        {
            var prescription = await _prescriptionRepo.GetByIdAsync(prescriptionId);

            if (prescription is null)
                return null;

            if (prescription.Appointment.Slot.DoctorId != doctorId)
                throw new UnauthorizedAccessException("You are not authorized to update this prescription");

            prescription.Diagnosis = dto.Diagnosis;
            prescription.UpdatedAt = DateTime.UtcNow;

            var existingMedicines = await _context.PrescriptionMedicines
                .Where(m => m.PrescriptionId == prescriptionId)
                .ToListAsync();

            _context.PrescriptionMedicines.RemoveRange(existingMedicines);

            foreach (var medDto in dto.Medicines)
            {
                var firstDoseTime = ParseTimeString(medDto.FirstDoseTime);
                ValidateMedicineDto(medDto, firstDoseTime);
                var medicine = new PrescriptionMedicine
                {
                    PrescriptionId = prescriptionId,
                    Name = medDto.Name,
                    Dose = medDto.Dose,
                    Duration = medDto.Duration,
                    TimesPerDay = medDto.TimesPerDay,
                    WhenToTake = medDto.WhenToTake,
                    FirstDoseTime = firstDoseTime,
                    DoseTimes = CalculateDoseTimes(firstDoseTime, medDto.TimesPerDay)
                };

                prescription.Medicines.Add(medicine);
            }

            await _prescriptionRepo.UpdateAsync(prescription);

            var loaded = await _prescriptionRepo.GetByIdAsync(prescriptionId);

            return MapToResponseDto(loaded!);
        }

        #endregion

        #region Dose Time Calculation

        private List<MedicineDoseTime> CalculateDoseTimes(TimeSpan firstDoseTime, TimesPerDay timesPerDay)
        {
            var doseTimes = new List<MedicineDoseTime>();
            int count = (int)timesPerDay;
            int intervalHours = 24 / count;

            for (int i = 0; i < count; i++)
            {
                var doseTime = firstDoseTime + TimeSpan.FromHours(intervalHours * i);

                if (doseTime.TotalHours >= 24)
                    doseTime -= TimeSpan.FromHours(24);

                doseTimes.Add(new MedicineDoseTime
                {
                    DoseTime = doseTime,
                    DoseOrder = i + 1
                });
            }

            return doseTimes;
        }

        private TimeSpan ParseTimeString(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                throw new InvalidOperationException("FirstDoseTime is required");

            if (TimeSpan.TryParse(timeString, out var result))
                return result;

            if (TimeSpan.TryParse($"{timeString}:00", out result))
                return result;

            if (int.TryParse(timeString, out var hours) && hours >= 0 && hours < 24)
                return TimeSpan.FromHours(hours);

            throw new InvalidOperationException($"Invalid time format: '{timeString}'. Expected formats: '12:00:00', '12:00', or '12'");
        }

        #endregion

        #region Validation

        private void ValidateMedicineDto(CreateMedicineDto med, TimeSpan firstDoseTime)
        {
            if (med.Duration <= 0)
                throw new InvalidOperationException("Duration must be greater than 0 days");
            if (!Enum.IsDefined(typeof(TimesPerDay), med.TimesPerDay))
                throw new InvalidOperationException($"Invalid TimesPerDay value: {med.TimesPerDay}");

            if (!Enum.IsDefined(typeof(WhenToTake), med.WhenToTake))
                throw new InvalidOperationException($"Invalid WhenToTake value: {med.WhenToTake}");

            if (firstDoseTime.TotalHours < 0 || firstDoseTime.TotalHours >= 24)
                throw new InvalidOperationException("FirstDoseTime must be a valid time of day (00:00 - 23:59)");
        }

        #endregion

        #region Mapping

        private PrescriptionResponseDto MapToResponseDto(Prescription prescription)
        {
            return new PrescriptionResponseDto
            {
                Id = prescription.Id,
                AppointmentId = prescription.AppointmentId,
                Diagnosis = prescription.Diagnosis,
                CreatedAt = prescription.CreatedAt,
                UpdatedAt = prescription.UpdatedAt,
                Medicines = prescription.Medicines
                    .OrderBy(m => m.Id)
                    .Select(MapToMedicineResponseDto)
                    .ToList()
            };
        }

        private MedicineResponseDto MapToMedicineResponseDto(PrescriptionMedicine medicine)
        {
            return new MedicineResponseDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                Dose = medicine.Dose,
                Duration = medicine.Duration,
                TimesPerDay = medicine.TimesPerDay,
                TimesPerDayDisplay = FormatTimesPerDay(medicine.TimesPerDay),
                WhenToTake = medicine.WhenToTake,
                WhenToTakeDisplay = FormatWhenToTake(medicine.WhenToTake),
                FirstDoseTime = medicine.FirstDoseTime,
                FirstDoseTimeDisplay = FormatTime(medicine.FirstDoseTime),
                DoseTimes = medicine.DoseTimes
                    .OrderBy(d => d.DoseOrder)
                    .Select(d => new DoseTimeResponseDto
                    {
                        DoseOrder = d.DoseOrder,
                        DoseTime = d.DoseTime,
                        DoseTimeDisplay = FormatTime(d.DoseTime)
                    })
                    .ToList()
            };
        }

        private static string FormatTimesPerDay(TimesPerDay t) => t switch
        {
            TimesPerDay.Once => "1x",
            TimesPerDay.Twice => "2x",
            TimesPerDay.ThreeTimes => "3x",
            TimesPerDay.FourTimes => "4x",
            _ => t.ToString()
        };

        private static string FormatWhenToTake(WhenToTake w) => w switch
        {
            WhenToTake.BeforeMeal => "Before Meal",
            WhenToTake.AfterMeals => "After Meals",
            WhenToTake.WithFood => "With Food",
            WhenToTake.AnyTime => "Any Time",
            _ => w.ToString()
        };

        private static string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }

        #endregion
    }
}
