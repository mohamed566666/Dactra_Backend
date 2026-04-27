namespace Dactra.Services.Implementation
{
    public class MedicalReportService : IMedicalReportService
    {
        private readonly IMedicalReportRepository _reportRepository;
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public MedicalReportService(
            IMedicalReportRepository reportRepository,
            IPatientProfileRepository patientProfileRepository,
            IFileService fileService,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _patientProfileRepository = patientProfileRepository;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<MedicalReportResponseDTO> UploadReportAsync(string userId, UploadMedicalReportDTO dto)
        {
            var profile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException("Patient Profile Not Found");
            var uploadResult = await _fileService.UploadAsync(dto.File, "medical-reports", 50);
            if (!uploadResult.Success)
                throw new InvalidOperationException(uploadResult.Message);

            var report = new MedicalReport
            {
                Name = dto.Name,
                FileUrl = uploadResult.FileUrl!,
                PublicId = uploadResult.PublicId!,
                FileType = uploadResult.ResourceType ?? "raw",
                PatientProfileId = profile.Id
            };

            await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();

            return _mapper.Map<MedicalReportResponseDTO>(report);
        }

        public async Task DeleteReportAsync(string userId, int reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
                throw new KeyNotFoundException("Medical Report Not Found");
            var profile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (profile == null || report.PatientProfileId != profile.Id)
                throw new UnauthorizedAccessException("You are not authorized to delete this report");

            await _fileService.DeleteAsync(report.PublicId);

            _reportRepository.Delete(report);
            await _reportRepository.SaveChangesAsync();
        }

        public async Task<List<MedicalReportResponseDTO>> GetMyReportsAsync(string userId)
        {
            var profile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException("Patient Profile Not Found");

            var reports = await _reportRepository.GetByPatientIdAsync(profile.Id);
            return _mapper.Map<List<MedicalReportResponseDTO>>(reports);
        }

        public async Task<List<MedicalReportResponseDTO>> GetReportsByPatientIdAsync(int patientProfileId)
        {
            var reports = await _reportRepository.GetByPatientIdAsync(patientProfileId);
            return _mapper.Map<List<MedicalReportResponseDTO>>(reports);
        }
    }
}
