namespace Dactra.Factories.Implementation
{
    public class UserProfileFactory : IUserProfileFactory
    {
        public ProfileBase CreateProfile(string role, string userId)
        {
            return role switch
            {
                "Patient" => new PatientProfile { UserId = userId },
                "Doctor" => new DoctorProfile { UserId = userId },
                "MedicalTestProvider" => new MedicalTestProviderProfile { UserId = userId },
                _ => throw new ArgumentException($"Role '{role}' is not recognized")
            };
        }
    }
}
