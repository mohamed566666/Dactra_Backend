namespace Dactra.Services.Interfaces
{
    public interface IPatientService
    {
        public Task CompleteRegistrationAsync(PatientCompleteDTO doctorComplateDTO);
        public Task<IEnumerable<PatientProfileResponseDTO>> GetAllProfileAsync();
        public Task DeletePatientProfileAsync(int patientProfileId);
        public Task<PatientProfileResponseDTO> GetProfileByUserID(string userId);
        public Task<PatientProfileResponseDTO> GetProfileByUserEmail(string email);
        public Task<PatientProfileResponseDTO> GetProfileByIdAsync(int patientProfileId);
        public Task UpdateProfileAsync(string userId, PatientUpdateDTO updatedProfile);
        Task<List<string>> GetAllergiesByPatientIdAsync(int patientId);
        Task<List<string>> GetChronicDiseasesByPatientIdAsync(int patientId);
        Task UpdateAllergiesAsync(string userId, List<int> allergyIds);
        Task UpdateChronicDiseasesAsync(string userId, List<int> chronicDiseaseIds);
        Task<List<string>> GetMyAllergiesAsync(string userId);
        Task<List<string>> GetMyChronicDiseasesAsync(string userId);
    }
}
