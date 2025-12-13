using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;

namespace Dactra.Services.Implementation
{
    public class DoctorQualificationService : IDoctorQualificationService
    {
        private readonly IDoctorQualificationRepository _repository;

        public DoctorQualificationService(IDoctorQualificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DoctorQualificationResponseDTO>> GetAllAsync(int doctorId)
        {
            var list = await _repository.GetByDoctorIdAsync(doctorId);
            return list.Select(q => new DoctorQualificationResponseDTO
            {
                Id = q.Id,
                Type = q.Type,
                Description = q.Description
            });
        }

        public async Task<bool> CreateAsync(int doctorId, DoctorQualificationDTO dto)
        {
            var qualification = new DoctorQualification
            {
                DoctorProfileId = doctorId,
                Type = dto.Type,
                Description = dto.Description
            };
            await _repository.AddAsync(qualification);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int qualificationId, DoctorQualificationDTO dto)
        {
            var qualification = await _repository.GetByIdAsync(qualificationId);
            if (qualification == null)
                return false;
            qualification.Type = dto.Type;
            qualification.Description = dto.Description;
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int qualificationId)
        {
            var qualification = await _repository.GetByIdAsync(qualificationId);
            if (qualification == null)
                return false;
            _repository.Delete(qualification);
            await _repository.SaveChangesAsync();
            return true;
        }
        public async Task CreateByUserIdAsync(string userId, DoctorQualificationDTO dto)
        {
            var doctor = await _repository.GetByUserIdAsync(userId)
                ?? throw new Exception("Doctor profile not found");
            var qualification = new DoctorQualification
            {
                DoctorProfileId = doctor.Id,
                Type = dto.Type,
                Description = dto.Description
            };

            await _repository.AddAsync(qualification);
            await _repository.SaveChangesAsync();
        }
        public async Task<bool> UpdateByUserIdAsync(string userId, int qualificationId, DoctorQualificationDTO dto)
        {
            var doctor = await _repository.GetByUserIdAsync(userId)
                ?? throw new Exception("Doctor profile not found");
            var qualification = await _repository.GetByIdAsync(qualificationId);
            if (qualification == null || qualification.DoctorProfileId != doctor.Id)
                return false;
            qualification.Type = dto.Type;
            qualification.Description = dto.Description;
            await _repository.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteByUserIdAsync(string userId, int qualificationId)
        {
            var doctor = await _repository.GetByUserIdAsync(userId)
                ?? throw new Exception("Doctor profile not found");
            var qualification = await _repository.GetByIdAsync(qualificationId);
            if (qualification == null || qualification.DoctorProfileId != doctor.Id)
                return false;
            _repository.Delete(qualification);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
