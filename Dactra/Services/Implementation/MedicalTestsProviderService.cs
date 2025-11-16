using Dactra.DTOs.ProfilesDTO;
using Dactra.Enums;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Dactra.Services.Implementation
{
    public class MedicalTestsProviderService : IMedicalTestsProviderService
    {
        private readonly IMedicalTestProviderProfileRepository _medicalTestProviderProfileRepository;
        private readonly IUserRepository _userRepository;
        public MedicalTestsProviderService(IMedicalTestProviderProfileRepository medicalTestProviderProfileRepository,IUserRepository userRepository)
        {
            _medicalTestProviderProfileRepository = medicalTestProviderProfileRepository;
            _userRepository = userRepository;
        }

        public async Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(medicalTestProviderDTO.Email);
            if (user == null)
            {
                throw new ArgumentException("User Not Found");
            }
            if (!user.IsVerified)
            {
                throw new InvalidOperationException("Please verify your account first");
            }
            var existingProfile = await _medicalTestProviderProfileRepository.GetByUserIdAsync(user.Id);
            if (existingProfile != null)
            {
                throw new InvalidOperationException("This User Already has an Profile");
            }
            var medicalTestProviderProfile = new MedicalTestProviderProfile
            {
                UserId = user.Id,
                User = user,
                Name = medicalTestProviderDTO.Name,
                LicenceNo = medicalTestProviderDTO.licenceNo,
                Address = medicalTestProviderDTO.Address,
                About = medicalTestProviderDTO.About,
                Type = medicalTestProviderDTO.rule.ToLower() == "lab" ? MedicalTestProviderType.Lab : MedicalTestProviderType.Scan
            };
            await _medicalTestProviderProfileRepository.AddAsync(medicalTestProviderProfile);
            user.IsRegistrationComplete = true;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task DeleteMedicalTestProviderProfileAsync(int medicalTestProviderProfileId)
        {
            var profile = await _medicalTestProviderProfileRepository.GetByIdAsync(medicalTestProviderProfileId);
            if (profile == null)
            {
                throw new ArgumentException("Medical Test Provider Profile Not Found");
            }
            await _medicalTestProviderProfileRepository.DeleteAsync(profile);
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetAllProfilesAsync()
        {
            return await _medicalTestProviderProfileRepository.GetAllAsync();
        }
    }
}
