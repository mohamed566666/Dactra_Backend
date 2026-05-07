using Google;
using System;

namespace Dactra.Services.Implementation
{
    public class MedicineReminderService : IMedicineReminderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientProfileRepository _patientRepo;

        public MedicineReminderService(
            ApplicationDbContext context,
            IPrescriptionService prescriptionService,
            IPatientProfileRepository patientRepo   )
        {
            _context = context;
            _prescriptionService = prescriptionService;
            _patientRepo = patientRepo;
        }
        public async Task<int> CreateFromPrescriptionAsync(int appointmentId)
        {
            var patientId = await _context.PatientAppointments
                .Where(a => a.Id == appointmentId)
                .Select(a => a.PatientId)
                .FirstOrDefaultAsync();

            var patientUserId = await _context.Patients
                .Where(p => p.Id == patientId)
                .Select(p => p.UserId)
                .FirstOrDefaultAsync();

            var prescription = await _prescriptionService.GetByAppointmentIdAsync(appointmentId, patientId, "Patient");


            var reminders = new List<MedicineReminder>();

            foreach (var med in prescription.Medicines)
            {
                var reminder = new MedicineReminder
                {
                    PatientId = patientUserId,

                    MedicineName = med.Name,
                    Dose = med.Dose,
                    MealRelation = med.WhenToTake,
                    Frequency = med.TimesPerDay,

                    ScheduledTimes = string.Join(",",
                                med.DoseTimes
                                    .Select(x => x.DoseTime.ToString(@"hh\:mm"))),

                    StartDate = DateTime.UtcNow,

                    EndDate = DateTime.UtcNow.AddDays(med.Duration),

                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                reminders.Add(reminder);
            }

            await _context.MedicineReminders.AddRangeAsync(reminders);
            await _context.SaveChangesAsync();

            return reminders.Count;
        }
    }
}
