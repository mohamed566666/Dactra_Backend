namespace Dactra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestServiceController : ControllerBase
    {
        private readonly ITestServiceService _testServiceRepository;

        public TestServiceController(ITestServiceService repository)
        {
            _testServiceRepository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TestServiceDto dto)
        {
            var result =await _testServiceRepository.CreateAsync(dto);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _testServiceRepository.GetAllAsync());
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok =await _testServiceRepository.DeleteAsync(id);
            return ok ? Ok("Deleted") : NotFound();
        }
        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result =await _testServiceRepository.GetByNameAsync(name);
            return result == null ? NotFound("TestService not found") : Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result =await _testServiceRepository.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TestServiceDto dto)
        {
            var ok =await _testServiceRepository.UpdateAsync(id, dto);
            return ok ? Ok("Updated") : NotFound();
        }
    }
}
