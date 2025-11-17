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
        private readonly ApplicationDbContext _context;
        public MedicalTestsProviderService(IMedicalTestProviderProfileRepository medicalTestProviderProfileRepository,IUserRepository userRepository, ApplicationDbContext context)
        {
            _medicalTestProviderProfileRepository = medicalTestProviderProfileRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
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
                    LicenceNo = medicalTestProviderDTO.LicenceNo,
                    Address = medicalTestProviderDTO.Address,
                    About = medicalTestProviderDTO.About,
                    Type = medicalTestProviderDTO.Role.ToLower() == "lab" ? MedicalTestProviderType.Lab : MedicalTestProviderType.Scan
                };
                await _medicalTestProviderProfileRepository.AddAsync(medicalTestProviderProfile);
                user.IsRegistrationComplete = true;
                await _userRepository.UpdateUserAsync(user);
                await transaction.CommitAsync();
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while completing registration: " + ex.Message);
            }
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
