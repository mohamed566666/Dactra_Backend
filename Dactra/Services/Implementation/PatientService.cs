using Dactra.DTOs.ProfilesDTO;
using Dactra.Models;
using Dactra.Repositories.Implementation;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using System.Data;

namespace Dactra.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public PatientService(IPatientProfileRepository patientProfileRepository, IUserRepository userRepository, ApplicationDbContext context)
        {
            _patientProfileRepository = patientProfileRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task CompleteRegistrationAsync(PatientCompleteDTO PatientCompleteDTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(PatientCompleteDTO.Email);
                if (user == null)
                {
                    throw new ArgumentException("User Not Found");
                }
                if (!user.IsVerified)
                {
                    throw new InvalidOperationException("Please verify your account first");
                }
                var existingProfile = await _patientProfileRepository.GetByUserIdAsync(user.Id);
                if (existingProfile != null)
                {
                    throw new InvalidOperationException("This User Already has an Profile");
                }
                var PatientProfile = new PatientProfile
                {
                    UserId = user.Id,
                    User = user,
                    FirstName = PatientCompleteDTO.FirstName,
                    LastName = PatientCompleteDTO.LastName,
                    Gender = PatientCompleteDTO.Gender,
                    Height = PatientCompleteDTO.Height,
                    Weight = PatientCompleteDTO.Weight,
                    DateOfBirth = PatientCompleteDTO.DateOfBirth,
                    BloodType = PatientCompleteDTO.BloodType,
                    IS_Smoking = PatientCompleteDTO.IS_Smoking,
                    Allergies = PatientCompleteDTO.Allergies,
                    MaritalStatus = PatientCompleteDTO.MaritalStatus,
                    ChronicDisease = PatientCompleteDTO.ChronicDisease,

                };

                await _patientProfileRepository.AddAsync(PatientProfile);
                user.IsRegistrationComplete = true;
                await _userRepository.UpdateUserAsync(user);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while completing registration: " + ex.Message);
            }
        }

        public async Task DeletePatientProfileAsync(int patientProfileId)
        {
            var profile =  await _patientProfileRepository.GetByIdAsync(patientProfileId);
            if (profile == null)
            {
                throw new ArgumentException("Patient Profile Not Found");
            }
            await _patientProfileRepository.DeleteAsync(profile);
        }

        public async Task<IEnumerable<PatientProfile>> GetAllProfileAsync()
        {
            return await _patientProfileRepository.GetAllAsync();
        }
    }
}
