namespace Dactra.Repositories.Interfaces
{
    public interface IMajorsRepository : IGenericRepository<Majors>
    {
        Task UpdateIcon(int id, string iconUrl);
    }
}
