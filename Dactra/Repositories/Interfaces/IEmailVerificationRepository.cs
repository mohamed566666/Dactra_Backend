namespace Dactra.Repositories.Interfaces
{
    public interface IEmailVerificationRepository
    {
        public Task AddVerificationAsync(string email, string code, TimeSpan validFor);
        public Task<EmailVerification?> GetByEmailAndCodeAsync(string email, string code);
        public Task<EmailVerification?> GetVerificationAsync(string email);
        public Task RemoveAsync(EmailVerification entity);
        public Task RemoveExpiredAsync();
    }
}
