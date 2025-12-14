using Dactra.DTOs.VitalSignDTOs;

namespace Dactra.Services.Implementation
{
    public class VitalSignService : IVitalSignService
    {
        private readonly IVitalSignRepository _repository;
        private readonly IPatientProfileRepository _patientRepository;
        private readonly IMapper _mapper;

        public VitalSignService(IVitalSignRepository repository, IPatientProfileRepository patientRepository, IMapper mapper)
        {
            _repository = repository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        private async Task<PatientProfile> GetPatientByUserIdAsync(string userId)
        {
            var patient = await _patientRepository.GetByUserIdAsync(userId);
            if (patient == null) throw new KeyNotFoundException("Patient not found");
            return patient;
        }

        public async Task<VitalSignResponseDTO> AddVitalSignAsync(string userId, VitalSignCreateDTO dto)
        {
            var patient = await GetPatientByUserIdAsync(userId);
            var type = await _repository.GetTypeByIdAsync(dto.VitalSignTypeId);
            if (type == null) throw new KeyNotFoundException("Vital sign type not found");

            if (type.IsComposite && (dto.Value2 == null))
                throw new InvalidOperationException("Composite vital sign requires two values");
            var entity = new VitalSign
            {
                PatientId = patient.Id,
                VitalSignTypeId = dto.VitalSignTypeId,
                Value = dto.Value,
                Value2 = dto.Value2,
                RecordedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return new VitalSignResponseDTO
            {
                VitalSignTypeId = entity.VitalSignTypeId,
                Value = entity.Value,
                Value2 = entity.Value2,
                date = entity.RecordedAt
            };
        }

        public async Task<List<VitalSignResponseDTO>> GetAllForPatientAsync(string userId)
        {
            var patient = await GetPatientByUserIdAsync(userId);
            var vitals = await _repository.GetByPatientIdAsync(patient.Id);
            return vitals.Select(v => new VitalSignResponseDTO
            {
                VitalSignTypeId = v.VitalSignTypeId,
                Value = v.Value,
                Value2 = v.Value2,
                date = v.RecordedAt
            }).ToList();
        }

        public async Task<bool> DeleteVitalSignAsync(string userId, int id)
        {
            var patient = await GetPatientByUserIdAsync(userId);
            var vital = await _repository.GetByIdAsync(id);
            if (vital == null || vital.PatientId != patient.Id) return false;
            _repository.Delete(vital);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<List<VitalSignType>> GetAllTypesAsync() => await _repository.GetAllTypesAsync();

        public async Task<VitalSignType> AddTypeAsync(string name, bool isComposite, string? compositeFields)
        {
            var existing = await _repository.GetTypeByNameAsync(name);
            if (existing != null) throw new InvalidOperationException("Type already exists");
            var type = new VitalSignType
            {
                Name = name,
                IsComposite = isComposite,
                CompositeFields = compositeFields
            };
            await _repository.AddTypeAsync(type);
            await _repository.SaveChangesAsync();
            return type;
        }
        public async Task<List<VitalSignResponseDTO>> GetByPatientIdAsync(int patientId)
        {
            var vitals = await _repository.GetByPatientIdAsync(patientId);
            return vitals.Select(v => new VitalSignResponseDTO
            {
                VitalSignTypeId = v.VitalSignTypeId,
                Value = v.Value,
                Value2 = v.Value2,
                date = v.RecordedAt
            }).ToList();
        }
    }
}
