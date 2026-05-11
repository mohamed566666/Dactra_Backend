namespace Dactra.Services.Interfaces
{
    public interface IpharmacyIntegration
    {
        public Task<string> CheckPharmacyAsync( int prescriptionId,string street,string city,string governorate);
    }
}
