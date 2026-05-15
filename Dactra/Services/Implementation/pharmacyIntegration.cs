
using Dactra.DTOs.PharmacyDto;
using System.Net.Http.Json;

namespace Dactra.Services.Implementation
{
    public class pharmacyIntegration : IpharmacyIntegration
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public pharmacyIntegration(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }
        public async Task<string> CheckPharmacyAsync(
             int prescriptionId,
             string street,
             string city,
             string governorate)
        {
            var prescriptionExists = await _context.Prescriptions
                .AnyAsync(p => p.Id == prescriptionId);
            if (!prescriptionExists)
            {
                throw new Exception("Prescription not found.");
            }
                var prescription = await _context.PrescriptionMedicines
                .Where(pm => pm.PrescriptionId == prescriptionId)
                .Select(pm => new PharmacyItemDto
                {
                    medicineName = pm.Name,
                    quantity = 1
                })
                .ToListAsync();
            if (prescription == null || !prescription.Any())
            {
                throw new Exception("No medicines found for this prescription.");
            }

            var requestBody = new PharmacyRequestDto
            {
                Street = street,
                City = city,
                Governorate = governorate,
                items = prescription
            };

            var response = await _httpClient.PostAsJsonAsync(
                   "api/PublicPharmacy/nearest",
                requestBody);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status: {response.StatusCode}, Response: {content}");
            }
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
