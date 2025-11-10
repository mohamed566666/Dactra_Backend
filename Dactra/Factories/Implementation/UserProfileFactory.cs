using Dactra.Factories.Interfaces;
using Dactra.Models;

namespace Dactra.Factories.Implementation
{
    public class UserProfileFactory : IUserProfileFactory
    {
        public object CreateProfile(string role, string userId)
        {
            return role switch
            {
                "PatientProfile" => new PatientProfile { UserId = userId },
                "DoctorProfile" => new DoctorProfile { UserId = userId },
                "MedicalTestProviderProfile" => new MedicalTestProviderProfile { UserId = userId },
                _ => throw new ArgumentException($"Role '{role}' is not recognized")
            };
        }
    }
}
