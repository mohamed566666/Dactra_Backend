namespace Dactra.Services.Implementation
{
    public class MajorsService : IMajorsService
    {
        private readonly IMajorsRepository  _majorsRepository;
        private readonly IMapper _mapper;

        public MajorsService(IMajorsRepository majorsRepository , IMapper mapper)
        {
            _majorsRepository = majorsRepository;
            _mapper = mapper;
        }

        public async Task CreateMajorAsync(MajorBasicsDTO majorDto)
        {
            var major = _mapper.Map<Majors>(majorDto);
            await _majorsRepository.AddAsync(major);
        }

        public async Task DeleteMajorByIdAsync(int id)
        {
            var major = await _majorsRepository.GetByIdAsync(id);
            if (major == null)
            {
                throw new KeyNotFoundException($"Major with ID {id} not found.");
            }
            _majorsRepository.Delete(major);
            await _majorsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<MajorsResponseDTO>> GetAllMajorsAsync()
        {
            var majors = await _majorsRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MajorsResponseDTO>>(majors);
        }

        public async Task<MajorsResponseDTO> GetMajorByIdAsync(int id)
        {
            var major = await _majorsRepository.GetByIdAsync(id);
            return _mapper.Map<MajorsResponseDTO>(major);
        }

        public async Task UpdateMajorAsync(int id , MajorBasicsDTO majorDto)
        {
            var existingMajor = await _majorsRepository.GetByIdAsync(id);
            if (existingMajor == null)
            {
                throw new KeyNotFoundException($"Major with ID {id} not found.");
            }
            _mapper.Map(majorDto, existingMajor);
            _majorsRepository.Update(existingMajor);
            await _majorsRepository.SaveChangesAsync();
        }

        public async Task UpdateMajorIconAsync(int id, string iconUrl)
        {
            var existingMajor = await _majorsRepository.GetByIdAsync(id);
            if (existingMajor == null)
            {
                throw new KeyNotFoundException($"Major with ID {id} not found.");
            }
            await _majorsRepository.UpdateIcon(id, iconUrl);
            await _majorsRepository.SaveChangesAsync();
        }
    }
}
