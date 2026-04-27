namespace Dactra.Mappings
{
    public class MedicalReportMapper : Profile
    {
        public MedicalReportMapper()
        {
            CreateMap<MedicalReport, MedicalReportResponseDTO>();
        }
    }
}
