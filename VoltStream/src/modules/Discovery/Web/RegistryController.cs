namespace Discovery.Web;

using Discovery.Models;
using Discovery.Store;
using Microsoft.AspNetCore.Mvc;

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
