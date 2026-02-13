
using System.Security.Claims;
namespace Dactra.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetDoctorId(this ClaimsPrincipal user)
        {
            var doctorIdClaim = user.FindFirst("DoctorId");

            if (doctorIdClaim == null)
                throw new UnauthorizedAccessException("DoctorId claim not found");

            return int.Parse(doctorIdClaim.Value);
        }
        public static int GetPatientId(this ClaimsPrincipal user)
        {
            var PatientIdClaim = user.FindFirst("PationtId");

            if (PatientIdClaim == null)
                throw new UnauthorizedAccessException("DoctorId claim not found");

            return int.Parse(PatientIdClaim.Value);
        }
    }
}
