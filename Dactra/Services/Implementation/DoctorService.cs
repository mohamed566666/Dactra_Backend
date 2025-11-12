using Dactra.DTOs.ProfilesDTO;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IUserRepository _userRepository;

        public DoctorService(IDoctorProfileRepository doctorProfileRepository , IUserRepository userRepository)
        {
            _doctorProfileRepository = doctorProfileRepository;
            _userRepository = userRepository;
        }

        public async Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO)
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
                FirstName = doctorComplateDTO.FirstName,
                LastName = doctorComplateDTO.LastName,
                LicenceNo = doctorComplateDTO.LicenceNo,
                DateOfBirth = doctorComplateDTO.DateOfBirth,
                StartingCareerDate = doctorComplateDTO.StartingCareerDate,
                Address = doctorComplateDTO.Address,
                Gender = doctorComplateDTO.Gender,
                specialization = doctorComplateDTO.Major,
                About = doctorComplateDTO.About
            };
            await _doctorProfileRepository.AddAsync(doctorProfile);
        }

        public async Task<IEnumerable<DoctorProfile>> GetAllProfileAsync()
        {
            return await _doctorProfileRepository.GetAllAsync();
        }
    }
}
