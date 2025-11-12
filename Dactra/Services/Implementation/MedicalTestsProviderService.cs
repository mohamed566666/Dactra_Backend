using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Dactra.Services.Implementation
{
    public class MedicalTestsProviderService : IMedicalTestsProviderService
    {
        private readonly IMedicalTestProviderProfileRepository _medicalTestProviderProfileRepository;
        public MedicalTestsProviderService(IMedicalTestProviderProfileRepository medicalTestProviderProfileRepository )
        {
            _medicalTestProviderProfileRepository = medicalTestProviderProfileRepository;
        }
    }
}
