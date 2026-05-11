using Dactra.DTOs.ProfilesDTOs.DoctorDTOs;
using Dactra.DTOs.RatingDTOs;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IDoctorQualificationService _doctorQualificationService;
        private readonly IUserRepository _userRepository;
        private readonly IRatingService _ratingService;
        private readonly IFavoriteService _favoriteService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DoctorService(
            IDoctorProfileRepository doctorProfileRepository,
            IRatingService ratingService,
            IUserRepository userRepository,
            ApplicationDbContext context,
            IDoctorQualificationService doctorQualificationService,
            IMapper mapper,
            IFavoriteService favoriteService)
        {
            _doctorProfileRepository = doctorProfileRepository;
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _doctorQualificationService = doctorQualificationService;
            _ratingService = ratingService;
            _favoriteService = favoriteService;
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
                doctorProfile.approvalStatus = ApprovalStatus.newPending;
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

        public async Task<IEnumerable<DoctorProfileResponseDTO>> GetAllProfileAsync(int patientId = 0)
        {
            var profiles = await _doctorProfileRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<DoctorProfileResponseDTO>>(profiles).ToList();

            if (patientId > 0 && dtos.Any())
            {
                var doctorIds = dtos.Select(d => d.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, doctorIds);
                foreach (var dto in dtos)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }

            return dtos;
        }

        public async Task<DoctorsResponseDTO> GetProfileByIdAsync(int doctorProfileId, int patientId = 0)
        {
            var profile = await _doctorProfileRepository.GetByIdAsync(doctorProfileId);
            if (profile == null)
                return null;

            var doctorDTO = _mapper.Map<DoctorsResponseDTO>(profile);
            var qualificationResponses = await _doctorQualificationService.GetAllAsync(profile.Id);
            doctorDTO.profileImageUrl = profile.User.ImageUrl;
            doctorDTO.Qualifications = qualificationResponses
                .Select(q => new DoctorQualificationDTO
                {
                    Type = q.Type,
                    Description = q.Description
                }).ToList();

            doctorDTO.ratings = await _ratingService.GetRatingsforProviderAsync(profile.Id);

            if (patientId > 0)
            {
                doctorDTO.IsFavorite = await _favoriteService.IsFavoriteAsync(patientId, profile.Id);
            }

            return doctorDTO;
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByUserEmail(string email, int patientId = 0)
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

            var dto = _mapper.Map<DoctorProfileResponseDTO>(profile);

            if (patientId > 0)
            {
                dto.IsFavorite = await _favoriteService.IsFavoriteAsync(patientId, profile.Id);
            }

            return dto;
        }

        public async Task<DoctorProfileResponseDTO> GetProfileByUserIdAsync(string userId)
        {
            var profile = await _doctorProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profile Not Found");
            }

            var dto = _mapper.Map<DoctorProfileResponseDTO>(profile);

            return dto;
        }

        public async Task UpdateProfileAsync(string userId, DoctorUpdateDTO updatedProfile)
        {
            var existingProfile = await _doctorProfileRepository.GetByUserIdAsync(userId);
            if (existingProfile == null)
            {
                throw new KeyNotFoundException("Profile Not Found");
            }
            existingProfile.FirstName = updatedProfile.FirstName;
            existingProfile.LastName = updatedProfile.LastName;
            existingProfile.Address = updatedProfile.Address;
            existingProfile.User.PhoneNumber = updatedProfile.PhoneNumber;
            existingProfile.About = updatedProfile.About;
            _doctorProfileRepository.Update(existingProfile);
            await _doctorProfileRepository.SaveChangesAsync();
        }

        public async Task<PaginatedDoctorsResponseDTO> GetFilteredDoctorsAsync(DoctorFilterDTO filter, int patientId = 0)
        {
            if (filter.PageNumber < 1)
                filter.PageNumber = 1;
            if (filter.PageSize < 1 || filter.PageSize > 100)
                filter.PageSize = 9;

            var (doctors, totalCount) = await _doctorProfileRepository.GetFilteredDoctorsAsync(filter);
            var result = _mapper.Map<PaginatedDoctorsResponseDTO>((doctors, totalCount, filter));

            if (patientId > 0 && result.Doctors.Any())
            {
                var doctorIds = result.Doctors.Select(d => d.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, doctorIds);

                foreach (var doc in result.Doctors)
                {
                    doc.IsFavorite = favoriteIds.Contains(doc.Id);
                }
            }

            return result;
        }

        public async Task<IEnumerable<DoctorProfileResponseDTO>> GetApprovedDoctorsAsync(int patientId = 0)
        {
            var doctors = await _doctorProfileRepository.GetApprovedDoctorsAsync();
            var dtos = _mapper.Map<IEnumerable<DoctorProfileResponseDTO>>(doctors).ToList();

            if (patientId > 0 && dtos.Any())
            {
                var doctorIds = dtos.Select(d => d.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, doctorIds);

                foreach (var dto in dtos)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<DoctorProfileResponseDTO>> GetdisApprovedDoctorsAsync(int patientId = 0)
        {
            var doctors = await _doctorProfileRepository.GetdisApprovedDoctorsAsync();
            var dtos = _mapper.Map<IEnumerable<DoctorProfileResponseDTO>>(doctors).ToList();

            if (patientId > 0 && dtos.Any())
            {
                var doctorIds = dtos.Select(d => d.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, doctorIds);

                foreach (var dto in dtos)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }

            return dtos;
        }
    }
}