namespace Dactra.Repositories.Implementation
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly ApplicationDbContext _context;
        public EmailVerificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddVerificationAsync(string email, string code, TimeSpan validFor)
        {
            var verification = new EmailVerification
            {
                Email = email,
                OTP = code,
                ExpiryDate = DateTime.UtcNow.Add(validFor)
            };
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync();
        }
        public async Task<EmailVerification?> GetByEmailAndCodeAsync(string email, string code)
        {
            return await _context.EmailVerifications
                .FirstOrDefaultAsync(ev => ev.Email == email && ev.OTP == code);
        }

        public async Task<EmailVerification?> GetVerificationAsync(string email)
        {
            return await _context.EmailVerifications
                .FirstOrDefaultAsync(ev => ev.Email == email);
        }
        public async Task RemoveAsync(EmailVerification entity)
        {
            _context.EmailVerifications.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveExpiredAsync()
        {
            var expiredVerifications = await _context.EmailVerifications
                .Where(ev => ev.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();
            _context.EmailVerifications.RemoveRange(expiredVerifications);
            await _context.SaveChangesAsync();
        }
    }   
}
