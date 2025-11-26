using Dactra.DTOs.MajorDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class MajorsService : IMajorsService
    {
        private readonly IMajorsRepository  _majorsRepository;

        public MajorsService(IMajorsRepository majorsRepository)
        {
            _majorsRepository = majorsRepository;
        }

        public async Task CreateMajorAsync(MajorBasicsDTO majorDto)
        {
           var major = new Majors
           {
               Name = majorDto.Name,
               Iconpath = majorDto.Iconpath,
               Description = majorDto.Description
           };
            await _majorsRepository.AddAsync(major);
        }

        public async Task DeleteMajorAsync(int id)
        {
            await _majorsRepository.DeleteAsync(new Majors { Id = id });
        }

        public async Task<IEnumerable<MajorsResponseDTO>> GetAllMajorsAsync()
        {
            var majors = await _majorsRepository.GetAllAsync();
            var majorDtos = majors.Select(m => new MajorsResponseDTO
            {
                Id = m.Id,
                Name = m.Name,
                IconPath = m.Iconpath,
                Description = m.Description
            });
            return majorDtos;
        }

        public async Task<MajorsResponseDTO> GetMajorByIdAsync(int id)
        {
            var major = await _majorsRepository.GetByIdAsync(id);
            var majorDto = new MajorsResponseDTO
            {
                Id = major.Id,
                Name = major.Name,
                IconPath = major.Iconpath,
                Description = major.Description
            };
            return majorDto;
        }

        public async Task UpdateMajorAsync(int id , MajorBasicsDTO major)
        {
            var existingMajor = await _majorsRepository.GetByIdAsync(id);
            if (existingMajor == null)
            {
                throw new KeyNotFoundException($"Major with ID {id} not found.");
            }
            existingMajor.Name = major.Name;
            existingMajor.Iconpath = major.Iconpath;
            existingMajor.Description = major.Description;
            await _majorsRepository.UpdateAsync(existingMajor);
        }

        public async Task UpdateMajorIconAsync(int id, string iconUrl)
        {
            var existingMajor = await _majorsRepository.GetByIdAsync(id);
            if (existingMajor == null)
            {
                throw new KeyNotFoundException($"Major with ID {id} not found.");
            }
            await _majorsRepository.UpdateIcon(id, iconUrl);
        }
    }
}
