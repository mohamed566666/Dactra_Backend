using AutoMapper;
using Dactra.DTOs.ProfilesDTOs.PatientDTOs;
using Dactra.Mappings;
using Dactra.Models;
using Dactra.Repositories.Implementation;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dactra.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IPatientProfileRepository _patientProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PatientService(IPatientProfileRepository patientProfileRepository, IUserRepository userRepository, ApplicationDbContext context , IMapper mapper )
        {
            _patientProfileRepository = patientProfileRepository;
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task CompleteRegistrationAsync(PatientCompleteDTO PatientCompleteDTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(PatientCompleteDTO.Email);
                if (user == null)
                {
                    throw new KeyNotFoundException("User Not Found");
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
                    SmokingStatus = PatientCompleteDTO.SmokingStatus,
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
            var profile = await _patientProfileRepository.GetByIdAsync(patientProfileId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Patient Profile Not Found");
            }
            await _patientProfileRepository.DeleteAsync(profile);
        }
        public async Task<IEnumerable<PatientProfileResponseDTO>> GetAllProfileAsync()
        {
            var profiles = await _patientProfileRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientProfileResponseDTO>>(profiles);
        }

        public async Task<PatientProfileResponseDTO> GetProfileByIdAsync(int patientProfileId)
        {
            var profile = await _patientProfileRepository.GetByIdAsync(patientProfileId);
            if (profile == null)
            {
                return null;
            }
            return _mapper.Map<PatientProfileResponseDTO>(profile);
        }

        public async Task<PatientProfileResponseDTO> GetProfileByUserEmail(string email)
        {
            var profile = await _patientProfileRepository.GetByUserEmail(email);
            if (profile == null)
            {
                return null;
            }
            return _mapper.Map<PatientProfileResponseDTO>(profile);
        }

        public async Task<PatientProfileResponseDTO> GetProfileByUserID(string userId)
        {
            var profile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Patient Profile Not Found");
            }
            return _mapper.Map<PatientProfileResponseDTO>(profile);
        }

        public async Task UpdateProfileAsync(string userId, PatientUpdateDTO updatedProfile)
        {
            var existingProfile = await _patientProfileRepository.GetByUserIdAsync(userId);
            if (existingProfile == null)
            {
                throw new KeyNotFoundException("Patient Profile Not Found");
            }
            existingProfile.FirstName = updatedProfile.FirstName;
            existingProfile.LastName = updatedProfile.LastName;
            existingProfile.Height = updatedProfile.Height;
            existingProfile.Weight = updatedProfile.Weight;
            existingProfile.BloodType = updatedProfile.BloodType;
            existingProfile.SmokingStatus = updatedProfile.SmokingStatus;
            existingProfile.MaritalStatus = updatedProfile.MaritalStatus;
            existingProfile.User.PhoneNumber = updatedProfile.PhoneNamber;
            existingProfile.AddressId = updatedProfile.AddressId;
            await _patientProfileRepository.UpdateAsync(existingProfile);
        }
    }
}
