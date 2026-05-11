using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyIntegrationController : ControllerBase
    {
        private readonly IpharmacyIntegration _pharmacyService;
        public PharmacyIntegrationController(IpharmacyIntegration ipharmacyIntegration)
        {
            _pharmacyService = ipharmacyIntegration;
        }
        [HttpPost("pharmacy")]
        public async Task<IActionResult> CheckPharmacy(
            int prescriptionId,
            string street,
            string city,
            string governorate)
        {
            var result = await _pharmacyService.CheckPharmacyAsync(
                prescriptionId,
                street,
                city,
                governorate);

            return Ok(result);
        }
    }
}
