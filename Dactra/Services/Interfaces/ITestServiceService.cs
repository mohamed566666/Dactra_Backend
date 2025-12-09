namespace Dactra.Services.Interfaces
{
    public interface ITestServiceService
    {
        Task<IEnumerable<TestService>> GetAllAsync();
        Task<TestService?> GetByIdAsync(int id);
        Task<TestService?> GetByNameAsync(string name);
        Task<TestService> CreateAsync(TestServiceDto dto);
        Task<bool> UpdateAsync(int id, TestServiceDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
