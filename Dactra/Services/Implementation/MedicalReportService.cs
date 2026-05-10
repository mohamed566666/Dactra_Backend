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

            var report = new MedicalReport
            {
                Name = dto.Name,
                Summary = dto.Summary,
                PatientProfileId = profile.Id
            };

            foreach (var file in dto.Files)
            {
                var uploadResult = await _fileService.UploadAsync(file, "medical-reports", 50);
                if (!uploadResult.Success)
                    throw new InvalidOperationException($"{file.FileName}: {uploadResult.Message}");

                report.Files.Add(new MedicalReportFile
                {
                    FileUrl = uploadResult.FileUrl!,
                    PublicId = uploadResult.PublicId!,
                    FileType = uploadResult.ResourceType ?? "raw"
                });
            }

            await _reportRepository.AddAsync(report);
            await _reportRepository.SaveChangesAsync();

            return new MedicalReportResponseDTO
            {
                Id = report.Id,
                Name = report.Name,
                Summary = report.Summary,
                UploadedAt = report.UploadedAt,
                Files = report.Files.Select(f => new MedicalReportFileResponseDTO
                {
                    Id = f.Id,
                    FileUrl = f.FileUrl,
                    FileType = f.FileType
                }).ToList()
            };
        }

        public async Task DeleteReportAsync(string userId, int reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
                throw new KeyNotFoundException("Medical Report Not Found");

            var profile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (profile == null || report.PatientProfileId != profile.Id)
                throw new UnauthorizedAccessException("You are not authorized to delete this report");

            foreach (var file in report.Files)
                await _fileService.DeleteAsync(file.PublicId);

            _reportRepository.Delete(report);
            await _reportRepository.SaveChangesAsync();
        }

        public async Task<List<MedicalReportResponseDTO>> GetMyReportsAsync(string userId)
        {
            var profile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException("Patient Profile Not Found");

            var reports = await _reportRepository.GetByPatientIdAsync(profile.Id);

            return reports.Select(r => new MedicalReportResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                Summary = r.Summary,
                UploadedAt = r.UploadedAt,
                Files = r.Files.Select(f => new MedicalReportFileResponseDTO
                {
                    Id = f.Id,
                    FileUrl = f.FileUrl,
                    FileType = f.FileType
                }).ToList()
            }).ToList();
        }

        public async Task<List<MedicalReportResponseDTO>> GetReportsByPatientIdAsync(int patientProfileId)
        {
            var reports = await _reportRepository.GetByPatientIdAsync(patientProfileId);

            return reports.Select(r => new MedicalReportResponseDTO
            {
                Id = r.Id,
                Name = r.Name,
                Summary = r.Summary,
                UploadedAt = r.UploadedAt,
                Files = r.Files.Select(f => new MedicalReportFileResponseDTO
                {
                    Id = f.Id,
                    FileUrl = f.FileUrl,
                    FileType = f.FileType
                }).ToList()
            }).ToList();
        }
    }
}