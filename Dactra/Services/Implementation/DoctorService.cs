using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public DoctorService(IDoctorProfileRepository doctorProfileRepository , IUserRepository userRepository, ApplicationDbContext context)
        {
            _doctorProfileRepository = doctorProfileRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(doctorComplateDTO.Email);
                if (user == null)
                {
                    throw new ArgumentException("User Not Found");
                }
                if (!user.IsVerified)
                {
                    throw new InvalidOperationException("Please verify your account first");
                }
                var existingProfile = await _doctorProfileRepository.GetByUserIdAsync(user.Id);
                if (existingProfile != null)
                {
                    throw new InvalidOperationException("This User Already has an Profile");
                }
                var doctorProfile = new DoctorProfile
                {
                    UserId = user.Id,
                    User = user,
                    FirstName = doctorComplateDTO.FirstName,
                    LastName = doctorComplateDTO.LastName,
                    LicenceNo = doctorComplateDTO.LicenceNo,
                    DateOfBirth = doctorComplateDTO.DateOfBirth,
                    StartingCareerDate = doctorComplateDTO.StartingCareerDate,
                    Address = doctorComplateDTO.Address,
                    Gender = doctorComplateDTO.Gender,
                    SpecializationId = doctorComplateDTO.MajorId,
                };
                await _doctorProfileRepository.AddAsync(doctorProfile);
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

        public async Task DeleteDoctorProfileAsync(int doctorProfileId)
        {
            var profile = await _doctorProfileRepository.GetByIdAsync(doctorProfileId);
            if (profile == null)
            {
                throw new ArgumentException("Doctor Profile Not Found");
            }
            await _doctorProfileRepository.DeleteAsync(profile);
        }

        public async Task<IEnumerable<DoctorProfileResponseDTO>> GetAllProfileAsync()
        {
            var profiles = await _doctorProfileRepository.GetAllAsync();
            return profiles.Select(profile => new DoctorProfileResponseDTO
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                About = profile.About,
                Address = profile.Address,
                age = DateTime.Now.Year - profile.DateOfBirth.Year,
                AverageRating = profile.Avg_Rating,
                YearsOfExperience = DateTime.Now.Year - profile.StartingCareerDate.Year,
                SpecializationId = profile.SpecializationId,
                gender = profile.Gender
            });
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByIdAsync(int doctorProfileId)
        {
            var profile = await _doctorProfileRepository.GetByIdAsync(doctorProfileId);
            if (profile == null)
            {
                throw new ArgumentException("Doctor Profile Not Found");
            }
            return new DoctorProfileResponseDTO
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                About = profile.About,
                Address = profile.Address,
                age = DateTime.Now.Year - profile.DateOfBirth.Year,
                AverageRating = profile.Avg_Rating,
                YearsOfExperience = DateTime.Now.Year - profile.StartingCareerDate.Year,
                SpecializationId = profile.SpecializationId,
            };
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByUserEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new ArgumentException("User Not Found");
            }
            var profile = await _doctorProfileRepository.GetByUserIdAsync(user.Id);
            if (profile == null)
            {
                throw new ArgumentException("Doctor Profile Not Found");
            }
            return new DoctorProfileResponseDTO
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                About = profile.About,
                Address = profile.Address,
                age = DateTime.Now.Year - profile.DateOfBirth.Year,
                AverageRating = profile.Avg_Rating,
                YearsOfExperience = DateTime.Now.Year - profile.StartingCareerDate.Year,
                SpecializationId = profile.SpecializationId,
            };
        }
    }
}
