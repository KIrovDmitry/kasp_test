using Kirov_DA_test.Models;
using Kirov_DA_test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kirov_DA_test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VulnerabilitiesController : ControllerBase
    {
        private readonly IVulnerabilityStore _store;

        public VulnerabilitiesController(IVulnerabilityStore store)
        {
            _store = store;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVulnerability([FromBody] Vulnerability vulnerability)
        {
            vulnerability.Id = Guid.NewGuid();
            vulnerability.CreatedAt = DateTime.UtcNow;

            await _store.AddAsync(vulnerability);
            return CreatedAtAction(nameof(GetVulnerability), new { id = vulnerability.Id }, vulnerability);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVulnerability(Guid id)
        {
            var vulnerability = await _store.GetByIdAsync(id);
            if (vulnerability == null) return NotFound();
            return Ok(vulnerability);
        }
    }
}