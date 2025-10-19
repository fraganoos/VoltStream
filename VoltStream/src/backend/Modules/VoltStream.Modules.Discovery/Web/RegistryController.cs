namespace VoltStream.Modules.Discovery.Web;

using Microsoft.AspNetCore.Mvc;
using VoltStream.Modules.Discovery.Models;
using VoltStream.Modules.Discovery.Store;

[ApiController]
[Route("api/discovery")]
public class RegistryController(IDiscoveryStore store) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register([FromBody] ServiceRegistration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.ServiceId))
            return BadRequest("ServiceId is required.");

        store.Register(registration);
        return Ok(new { message = "Registered successfully." });
    }

    [HttpGet("nodes/{serviceId}")]
    public IActionResult GetNodes(string serviceId)
    {
        var nodes = store.GetNodes(serviceId);
        return Ok(nodes);
    }
}
