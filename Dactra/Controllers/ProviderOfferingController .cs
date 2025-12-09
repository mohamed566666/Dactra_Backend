namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderOfferingController : ControllerBase
    {
        private readonly IProviderOfferingService _service;

        public ProviderOfferingController(IProviderOfferingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("provider/{providerId}")]
        public async Task<IActionResult> GetByProvider(int providerId)
            => Ok(await _service.GetByProviderIdAsync(providerId));

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetByService(int serviceId)
            => Ok(await _service.GetByServiceIdAsync(serviceId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProviderOfferingDto dto)
        {
            var entity = await _service.CreateAsync(dto);
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProviderOfferingDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok() : NotFound();
        }
    }
}
