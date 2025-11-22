using Dactra.DTOs;
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

        public async Task<Majors> CreateMajorAsync(MajorBasicsDTO majorDto)
        {
           var major = new Majors
           {
               Name = majorDto.Name,
               Iconpath = majorDto.Iconpath,
               Description = majorDto.Description
           };
            await _majorsRepository.AddAsync(major);
            return major;
        }

        public async Task DeleteMajorAsync(int id)
        {
            await _majorsRepository.DeleteAsync(new Majors { Id = id });
        }

        public async Task<IEnumerable<Majors>> GetAllMajorsAsync()
        {
            var majors = await _majorsRepository.GetAllAsync();
            return majors;
        }

        public async Task<Majors?> GetMajorByIdAsync(int id)
        {
            var major = await _majorsRepository.GetByIdAsync(id);
            return major;
        }

        public async Task<Majors?> UpdateMajorAsync(int id , MajorBasicsDTO major)
        {
            var existingMajor = await _majorsRepository.GetByIdAsync(id);
            if (existingMajor == null)
            {
                return null;
            }
            existingMajor.Name = major.Name;
            existingMajor.Iconpath = major.Iconpath;
            existingMajor.Description = major.Description;
            await _majorsRepository.UpdateAsync(existingMajor);
            return existingMajor;
        }

        public async Task UpdateMajorIconAsync(int id, string iconUrl)
        {
            await _majorsRepository.UpdateIcon(id, iconUrl);
        }
    }
}
