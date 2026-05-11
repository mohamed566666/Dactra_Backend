using Dactra.DTOs;
using Dactra.DTOs.ProfilesDTOs.MedicalTestsProviderDTOs;
using Dactra.Enums;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Services.Implementation
{
    public class MedicalTestsProviderService : IMedicalTestsProviderService
    {
        private readonly IMedicalTestProviderProfileRepository _medicalTestProviderProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteService _favoriteService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MedicalTestsProviderService(
            IMedicalTestProviderProfileRepository medicalTestProviderProfileRepository,
            IUserRepository userRepository,
            IFavoriteService favoriteService,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _medicalTestProviderProfileRepository = medicalTestProviderProfileRepository;
            _userRepository = userRepository;
            _favoriteService = favoriteService;
            _context = context;
            _mapper = mapper;
        }

        public async Task ApproveProfileAsync(int id)
        {
            var profile = await _medicalTestProviderProfileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                throw new KeyNotFoundException("Medical Test Provider Profile Not Found");
            }
            profile.approvalStatus = ApprovalStatus.approved;
            _medicalTestProviderProfileRepository.Update(profile);
            await _medicalTestProviderProfileRepository.SaveChangesAsync();
        }

        public async Task CompleteRegistrationAsync(MedicalTestProviderDTO medicalTestProviderDTO)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(medicalTestProviderDTO.Email);
                if (user == null)
                {
                    throw new KeyNotFoundException("User Not Found");
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
                var medicalTestProviderProfile = _mapper.Map<MedicalTestProviderProfile>(medicalTestProviderDTO);
                medicalTestProviderProfile.UserId = user.Id;
                await _medicalTestProviderProfileRepository.AddAsync(medicalTestProviderProfile);
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

        public async Task DeleteMedicalTestProviderProfileAsync(int medicalTestProviderProfileId)
        {
            var profile = await _medicalTestProviderProfileRepository.GetByIdAsync(medicalTestProviderProfileId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Medical Test Provider Profile Not Found");
            }
            _medicalTestProviderProfileRepository.Delete(profile);
            await _medicalTestProviderProfileRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetAllProfilesAsync(int patientId = 0)
        {
            var profiles = await _medicalTestProviderProfileRepository.GetAllAsync();
            var profileDTOs = _mapper.Map<IEnumerable<MedicalTestsProviderResponseDTO>>(profiles).ToList();

            if (patientId > 0 && profileDTOs.Any())
            {
                var providerIds = profileDTOs.Select(p => p.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, providerIds);

                foreach (var dto in profileDTOs)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }

            return profileDTOs;
        }

        public async Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null, int patientId = 0)
        {
            var profiles = await _medicalTestProviderProfileRepository.GetApprovedProfilesAsync(type);
            var profileDTOs = _mapper.Map<IEnumerable<MedicalTestsProviderResponseDTO>>(profiles).ToList();

            if (patientId > 0 && profileDTOs.Any())
            {
                var providerIds = profileDTOs.Select(p => p.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, providerIds);

                foreach (var dto in profileDTOs)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }

            return profileDTOs;
        }

        public async Task<MedicalTestsProviderResponseDTO> GetProfileByIdAsync(int id, int patientId = 0)
        {
            var profile = await _medicalTestProviderProfileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                throw new KeyNotFoundException("Medical Test Provider Profile Not Found");
            }
            var profileDTO = _mapper.Map<MedicalTestsProviderResponseDTO>(profile);

            if (patientId > 0)
            {
                profileDTO.IsFavorite = await _favoriteService.IsFavoriteAsync(patientId, profile.Id);
            }

            return profileDTO;
        }

        public async Task<MedicalTestsProviderResponseDTO> GetProfileByUserIdAsync(string userId)
        {
            var profile = await _medicalTestProviderProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Medical Test Provider Profile Not Found");
            }
            var profileDTO = _mapper.Map<MedicalTestsProviderResponseDTO>(profile);
            return profileDTO;
        }

        public async Task<IEnumerable<MedicalTestsProviderResponseDTO>> GetProfilesByTypeAsync(MedicalTestProviderType type, int patientId = 0)
        {
            var profiles = await _medicalTestProviderProfileRepository.GetProfilesByTypeAsync(type);
            var profileDTOs = _mapper.Map<IEnumerable<MedicalTestsProviderResponseDTO>>(profiles).ToList();

            if (patientId > 0 && profileDTOs.Any())
            {
                var providerIds = profileDTOs.Select(p => p.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, providerIds);

                foreach (var dto in profileDTOs)
                {
                    dto.IsFavorite = favoriteIds.Contains(dto.Id);
                }
            }

            return profileDTOs;
        }

        public async Task RejectProfileAsync(int id)
        {
            var profile = await _medicalTestProviderProfileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                throw new KeyNotFoundException("Medical Test Provider Profile Not Found");
            }
            profile.approvalStatus = ApprovalStatus.rejected;
            _medicalTestProviderProfileRepository.Update(profile);
            await _medicalTestProviderProfileRepository.SaveChangesAsync();
        }

        public async Task UpdateProfileAsync(string id, MedicalTestsProviderUpdateDTO dto)
        {
            var profile = await _context.MedicalTestProviders
                  .Include(x => x.WorkingHours)
                  .FirstOrDefaultAsync(x => x.UserId == id);
            if (profile == null)
            {
                throw new KeyNotFoundException("Medical Test Provider Profile Not Found");
            }
            _context.LabsWorkingHour.RemoveRange(profile.WorkingHours);
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                profile.User.PhoneNumber = dto.PhoneNumber;
            }
            _mapper.Map(dto, profile);
            _medicalTestProviderProfileRepository.Update(profile);
            await _medicalTestProviderProfileRepository.SaveChangesAsync();
        }

        public async Task<PagedResultDto<MedicalTestProviderSearchResultDTO>> SearchProvidersAsync(MedicalTestProviderSearchFilterDTO filter, int patientId = 0)
        {
            var (items, totalCount) = await _medicalTestProviderProfileRepository.SearchAsync(
                filter.SearchTerm,
                filter.Type,
                filter.Skip,
                filter.PageSize);

            var resultItems = _mapper.Map<IEnumerable<MedicalTestProviderSearchResultDTO>>(items).ToList();

            if (patientId > 0 && resultItems.Any())
            {
                var providerIds = resultItems.Select(p => p.Id).ToList();
                var favoriteIds = await _favoriteService.GetFavoriteServiceProviderIdsAsync(patientId, providerIds);

                foreach (var item in resultItems)
                {
                    item.IsFavorite = favoriteIds.Contains(item.Id);
                }
            }

            return new PagedResultDto<MedicalTestProviderSearchResultDTO>
            {
                Items = resultItems,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }
    }
}