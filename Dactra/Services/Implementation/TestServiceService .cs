namespace Dactra.Services.Implementation
{
    public class TestServiceService:ITestServiceService
    {
        private readonly ITestServiceRepository _repo;

        public TestServiceService(ITestServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TestService>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<TestService?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<TestService?> GetByNameAsync(string name)
        {
            return await _repo.GetByNameAsync(name);
        }

        public async Task<TestService> CreateAsync(TestServiceDto dto)
        {
            var service = await _repo.CreateAsync(dto);
            await _repo.SaveAsync();
            return service;
        }

        public async Task<bool> UpdateAsync(int id, TestServiceDto dto)
        {
            var updated = await _repo.UpdateAsync(id, dto);
            if (!updated) return false;

            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted) return false;

            await _repo.SaveAsync();
            return true;
        }
    }
}
