using AutoMapper;
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
        private readonly IMapper _mapper;

        public DoctorService(IDoctorProfileRepository doctorProfileRepository , IUserRepository userRepository, ApplicationDbContext context , IMapper mapper)
        {
            _doctorProfileRepository = doctorProfileRepository;
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task CompleteRegistrationAsync(DoctorCompleteDTO doctorComplateDTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(doctorComplateDTO.Email);
                if (user == null)
                {
                    throw new KeyNotFoundException("User Not Found");
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
                var doctorProfile = _mapper.Map<DoctorProfile>(doctorComplateDTO);
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
                throw new KeyNotFoundException("Doctor Profile Not Found");
            }
            _doctorProfileRepository.Delete(profile);
            await _doctorProfileRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<DoctorProfileResponseDTO>> GetAllProfileAsync()
        {
            var profiles = await _doctorProfileRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorProfileResponseDTO>>(profiles);
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByIdAsync(int doctorProfileId)
        {
            var profile = await _doctorProfileRepository.GetByIdAsync(doctorProfileId);
            if (profile == null)
            {
                return null;
            }
            return _mapper.Map<DoctorProfileResponseDTO>(profile);
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByUserEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException("User Not Found");
            }
            var profile = await _doctorProfileRepository.GetByUserIdAsync(user.Id);
            if (profile == null)
            {
                throw new KeyNotFoundException("Doctor Profile Not Found");
            }
            return _mapper.Map<DoctorProfileResponseDTO>(profile);
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByUserIdAsync(string userId)
        {
            var profile = await _doctorProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profile Not Found");
            }
            return _mapper.Map<DoctorProfileResponseDTO>(profile);
        }

        public async Task UpdateProfileAsync(string userId, DoctorUpdateDTO updatedProfile)
        {
            var existingProfile = await _doctorProfileRepository.GetByUserIdAsync(userId);
            if (existingProfile == null)
            {
                throw new KeyNotFoundException("Profile Not Found");
            }
            _mapper.Map(updatedProfile, existingProfile);
            _doctorProfileRepository.Update(existingProfile);
            await _doctorProfileRepository.SaveChangesAsync();
        }

        public async Task<PaginatedDoctorsResponseDTO> GetFilteredDoctorsAsync(DoctorFilterDTO filter)
        {
            if (filter.PageNumber < 1)
                filter.PageNumber = 1;
            if (filter.PageSize < 1 || filter.PageSize > 100)
                filter.PageSize = 9;
            var (doctors, totalCount) = await _doctorProfileRepository.GetFilteredDoctorsAsync(filter);
            var doctorDTOs = doctors.Select(d => new DoctorsFilterResponseDTO
            {
                Id = d.Id,
                Name = $"{d.FirstName} {d.LastName}",
                Specialization = d.specialization?.Name ?? "N/A",
                AverageRating = d.Avg_Rating
            });
            var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);
            return new PaginatedDoctorsResponseDTO
            {
                Doctors = doctorDTOs,
                CurrentPage = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
