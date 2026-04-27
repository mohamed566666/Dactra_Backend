namespace Dactra.Services.Interfaces
{
    public interface IMedicalReportService
    {
        Task<MedicalReportResponseDTO> UploadReportAsync(string userId, UploadMedicalReportDTO dto);
        Task DeleteReportAsync(string userId, int reportId);
        Task<List<MedicalReportResponseDTO>> GetMyReportsAsync(string userId);
        Task<List<MedicalReportResponseDTO>> GetReportsByPatientIdAsync(int patientProfileId);
    }
}
